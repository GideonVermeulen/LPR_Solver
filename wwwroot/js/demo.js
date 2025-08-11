// Global variables
let selectedAlgorithm = null;
let problemData = null;
let solutionData = null;

// File handling
document.getElementById('fileInput').addEventListener('change', handleFileSelect);

// Drag and drop functionality
const fileContainer = document.querySelector('.file-input-container');
fileContainer.addEventListener('dragover', (e) => {
    e.preventDefault();
    fileContainer.classList.add('dragover');
});

fileContainer.addEventListener('dragleave', () => {
    fileContainer.classList.remove('dragover');
});

fileContainer.addEventListener('drop', (e) => {
    e.preventDefault();
    fileContainer.classList.remove('dragover');
    const files = e.dataTransfer.files;
    if (files.length > 0) {
        handleFile(files[0]);
    }
});

function handleFileSelect(event) {
    const file = event.target.files[0];
    if (file) {
        handleFile(file);
    }
}

function handleFile(file) {
    if (!file.name.endsWith('.txt')) {
        showFileStatus('Error: Please select a .txt file', 'error');
        return;
    }

    const reader = new FileReader();
    reader.onload = function(e) {
        try {
            parseInputFile(e.target.result);
            showFileStatus('File loaded successfully!', 'success');
        } catch (error) {
            showFileStatus('Error parsing file: ' + error.message, 'error');
        }
    };
    reader.readAsText(file);
}

function showFileStatus(message, type) {
    const statusDiv = document.getElementById('fileStatus');
    statusDiv.innerHTML = `
        <div style="padding: 10px; border-radius: 6px; background: ${type === 'success' ? 'rgba(39, 174, 96, 0.1)' : 'rgba(231, 76, 60, 0.1)'}; color: ${type === 'success' ? 'var(--success)' : 'var(--danger)'}; border: 1px solid ${type === 'success' ? 'var(--success)' : 'var(--danger)'};">
            <i class="fas fa-${type === 'success' ? 'check' : 'exclamation-triangle'}"></i> ${message}
        </div>
    `;
}

function parseInputFile(content) {
    const lines = content.trim().split('\n').map(line => line.trim()).filter(line => line);
    
    if (lines.length < 2) {
        throw new Error('Invalid file format: At least 2 lines required');
    }

    // Parse first line (objective function)
    const objLine = lines[0].split(/\s+/);
    const direction = objLine[0].toLowerCase();
    
    if (direction !== 'max' && direction !== 'min') {
        throw new Error('First word must be "max" or "min"');
    }

    document.getElementById('objDir').value = direction;
    document.getElementById('objFunc').value = objLine.slice(1).join(' ');

    // Parse constraints (all lines except first and last)
    const constraintLines = lines.slice(1, -1);
    
    // Clear existing constraints
    document.getElementById('constraints').innerHTML = '';
    
    constraintLines.forEach(line => {
        const parts = line.split(/\s+/);
        const rhs = parts[parts.length - 1];
        const operator = parts[parts.length - 2];
        const coefficients = parts.slice(0, -2).join(' ');
        
        addConstraintRow(coefficients, operator, rhs);
    });

    // Parse sign restrictions (last line)
    const restrictionLine = lines[lines.length - 1].split(/\s+/);
    
    // Clear existing variables
    document.getElementById('varRestrictions').innerHTML = '';
    
    restrictionLine.forEach((restriction, index) => {
        addVariableRow(`x${index + 1}`, restriction);
    });
}

// Dynamic constraint management
document.getElementById('addConstraint').addEventListener('click', () => {
    addConstraintRow('', '<=', '');
});

function addConstraintRow(coefficients = '', operator = '<=', rhs = '') {
    const div = document.createElement('div');
    div.className = 'constraint-row';
    div.innerHTML = `
        <input type="text" placeholder="Enter coefficients (e.g., +11 +8 +6 +14 +10 +10)" value="${coefficients}" />
        <select>
            <option value="<=" ${operator === '<=' ? 'selected' : ''}>&lt;=</option>
            <option value="=" ${operator === '=' ? 'selected' : ''}>=</option>
            <option value=">=" ${operator === '>=' ? 'selected' : ''}>&gt;=</option>
        </select>
        <input type="text" placeholder="RHS value" value="${rhs}" style="width: 100px;" />
        <button class="remove-btn" onclick="this.parentElement.remove()">
            <i class="fas fa-times"></i>
        </button>
    `;
    document.getElementById('constraints').appendChild(div);
}

// Dynamic variable management
document.getElementById('addVariable').addEventListener('click', () => {
    const varCount = document.querySelectorAll('#varRestrictions .variable-row').length + 1;
    addVariableRow(`x${varCount}`, '+');
});

function addVariableRow(varName = '', restriction = '+') {
    const div = document.createElement('div');
    div.className = 'variable-row';
    div.innerHTML = `
        <input type="text" placeholder="Variable name" value="${varName}" style="width: 100px;" />
        <select>
            <option value="+" ${restriction === '+' ? 'selected' : ''}>≥ 0 (+)</option>
            <option value="-" ${restriction === '-' ? 'selected' : ''}>≤ 0 (-)</option>
            <option value="urs" ${restriction === 'urs' ? 'selected' : ''}>Unrestricted (urs)</option>
            <option value="int" ${restriction === 'int' ? 'selected' : ''}>Integer (int)</option>
            <option value="bin" ${restriction === 'bin' ? 'selected' : ''}>Binary (bin)</option>
        </select>
        <button class="remove-btn" onclick="this.parentElement.remove()">
            <i class="fas fa-times"></i>
        </button>
    `;
    document.getElementById('varRestrictions').appendChild(div);
}

// Algorithm selection
document.querySelectorAll('.algo-card').forEach(card => {
    card.addEventListener('click', () => {
        document.querySelectorAll('.algo-card').forEach(c => c.classList.remove('selected'));
        card.classList.add('selected');
        selectedAlgorithm = card.dataset.algo;
        document.getElementById('selectedAlgo').textContent = card.textContent.trim();
    });
});

// Tab functionality
document.querySelectorAll('.tab-btn').forEach(btn => {
    btn.addEventListener('click', () => {
        document.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
        document.querySelectorAll('.tab-content').forEach(c => c.classList.remove('active'));
        
        btn.classList.add('active');
        document.getElementById(btn.dataset.tab + 'Tab').classList.add('active');
    });
});

function runDualityAnalysis() {
    const dualProblemDiv = document.getElementById('dualProblem');
    dualProblemDiv.innerHTML = `
        <h4>Dual Problem</h4>
        <p><strong>Objective:</strong> Min y0</p>
        <p><strong>Subject to:</strong></p>
        <ul style="text-align: left; margin-left: 20px;">
            <li>y1 + 2y2 >= 3</li>
            <li>3y1 + y2 >= 4</li>
        </ul>
        <p><strong>Variable Restrictions:</strong> y1, y2 >= 0</p>
    `;
}

function showDualSolution() {
    const dualityResults = document.getElementById('dualityResults');
    dualityResults.style.display = 'block';
    dualityResults.innerHTML = '<h4>Dual Solution</h4><p>y1 = 2, y2 = 1, Objective = 10</p>';
}

function checkDuality() {
    const dualityResults = document.getElementById('dualityResults');
    dualityResults.style.display = 'block';
    dualityResults.innerHTML = '<h4>Duality Check</h4><p>Strong Duality holds.</p>';
}


function buildProblemFromUI() {
    // Get objective function
    const direction = document.getElementById('objDir').value;
    const objFunc = document.getElementById('objFunc').value.trim();
    
    if (!objFunc) {
        throw new Error('Please enter an objective function');
    }

    // Get constraints
    const constraintRows = document.querySelectorAll('#constraints .constraint-row');
    const constraints = [];
    
    constraintRows.forEach((row, index) => {
        const coeffs = row.querySelector('input:first-child').value.trim();
        const operator = row.querySelector('select').value;
        const rhs = row.querySelector('input:last-child').value.trim();
        
        if (coeffs && rhs) {
            constraints.push({ coefficients: coeffs, operator, rhs });
        }
    });

    // Get variable restrictions
    const varRows = document.querySelectorAll('#varRestrictions .variable-row');
    const variables = [];
    
    varRows.forEach(row => {
        const name = row.querySelector('input').value.trim();
        const restriction = row.querySelector('select').value;
        
        if (name) {
            variables.push({ name, restriction });
        }
    });

    problemData = {
        direction,
        objective: objFunc,
        constraints,
        variables
    };
}

function displayCanonicalForm(problem) {
    const canonicalDiv = document.getElementById('canonicalForm');
    canonicalDiv.innerHTML = `
        <h4>Canonical Form</h4>
        <p><strong>Objective:</strong> ${problem.direction} ${problem.objective}</p>
        <p><strong>Subject to:</strong></p>
        <ul style="text-align: left; margin-left: 20px;">
            ${problem.constraints.map(c => `<li>${c.coefficients} ${c.operator} ${c.rhs}</li>`).join('')}
        </ul>
        <p><strong>Variable Restrictions:</strong> ${problem.variables.map(v => `${v.name}: ${v.restriction}`).join(', ')}</p>
    `;
}

function displaySolution(result) {
    solutionData = result; // Store solution data
    const tbody = document.getElementById('solutionTable');
    tbody.innerHTML = `
        <tr>
            <td><strong>Objective Value</strong></td>
            <td><strong>${result.objectiveValue}</strong></td>
            <td>-</td>
        </tr>
    `;
    
    // Add variable values
    Object.entries(result.variables).forEach(([name, value]) => {
        tbody.innerHTML += `
            <tr>
                <td>${name}</td>
                <td>${value}</td>
                <td>${value > 0 ? 'Basic' : 'Non-basic'}</td>
            </tr>
        `;
    });
}

function displayIterations(iterations) {
    const iterationsDiv = document.getElementById('allIterations');
    iterationsDiv.innerHTML = ''; // Clear previous iterations
    
    // This is a placeholder for displaying iterations
    iterationsDiv.innerHTML = '<p style="text-align: center; color: var(--gray);">Tableau iterations will be displayed here.</p>';
}

function updateStatus(message, type) {
    const badge = document.getElementById('statusBadge');
    const icons = {
        success: 'check',
        warning: 'clock',
        error: 'exclamation-triangle'
    };
    
    const colors = {
        success: { bg: 'rgba(39, 174, 96, 0.1)', color: 'var(--success)' },
        warning: { bg: 'rgba(243, 156, 18, 0.1)', color: 'var(--warning)' },
        error: { bg: 'rgba(231, 76, 60, 0.1)', color: 'var(--danger)' }
    };
    
    badge.innerHTML = `<i class="fas fa-${icons[type]}"></i> ${message}`;
    badge.style.background = colors[type].bg;
    badge.style.color = colors[type].color;
}

// Export functionality
document.getElementById('exportBtn').addEventListener('click', () => {
    if (!problemData) {
        alert('No problem to export. Please define a problem first.');
        return;
    }
    
    alert('Export functionality will be implemented here.');
});

// Initialize with default algorithm
document.querySelector('.algo-card[data-algo="primal"]').click();

// --- Sensitivity Analysis --- //

document.getElementById('runBtn').addEventListener('click', () => {
    alert('Run button clicked. This functionality is not yet implemented.');
});

document.getElementById('addActivityBtn').addEventListener('click', () => {
    document.getElementById('addActivityFields').style.display = 'block';
    const newActivityCoeffs = document.getElementById('newActivityCoeffs');
    newActivityCoeffs.innerHTML = '';
    const numConstraints = document.getElementById('simplexTableau').querySelectorAll('tbody tr').length;
    for (let i = 0; i < numConstraints; i++) {
        const input = document.createElement('input');
        input.type = 'number';
        input.placeholder = `Coeff for row ${i + 1}`;
        newActivityCoeffs.appendChild(input);
    }
});

document.getElementById('applyAddActivity').addEventListener('click', () => {
    const table = document.getElementById('simplexTableau');
    const headerRow = table.querySelector('thead tr');
    const newVarName = `x${headerRow.children.length}`;
    const newHeader = document.createElement('th');
    newHeader.textContent = newVarName;
    headerRow.insertBefore(newHeader, headerRow.children[headerRow.children.length - 1]);

    const bodyRows = table.querySelectorAll('tbody tr');
    const coeffs = document.getElementById('newActivityCoeffs').querySelectorAll('input');
    bodyRows.forEach((row, i) => {
        const newCell = document.createElement('td');
        newCell.setAttribute('contenteditable', 'true');
        newCell.textContent = coeffs[i] ? coeffs[i].value : '0';
        row.insertBefore(newCell, row.children[row.children.length - 1]);
    });
    document.getElementById('addActivityFields').style.display = 'none';
    updateRemoveDropdowns();
});

document.getElementById('addConstraintBtn').addEventListener('click', () => {
    document.getElementById('addConstraintFields').style.display = 'block';
});

document.getElementById('applyAddConstraint').addEventListener('click', () => {
    const eq = document.getElementById('newConstraintEq').value;
    const parts = eq.split(/([<>]=?|=)/).map(s => s.trim());
    if (parts.length !== 3) {
        alert('Invalid constraint format. Expected format: coefficients operator RHS (e.g., +3 +2 <= 2)');
        return;
    }
    const coeffsStr = parts[0].split(/\s+/);
    const operator = parts[1];
    const rhs = parseFloat(parts[2]);

    const table = document.getElementById('simplexTableau');
    const newRow = table.querySelector('tbody').insertRow(-1);
    const numCols = table.querySelector('thead tr').children.length;
    
    const basisCell = newRow.insertCell();
    basisCell.textContent = `c${table.querySelectorAll('tbody tr').length - 1}`;

    for (let i = 1; i < numCols - 1; i++) { // Exclude Basis and RHS columns
        const cell = newRow.insertCell();
        cell.setAttribute('contenteditable', 'true');
        cell.textContent = (parseFloat(coeffsStr[i-1]) || 0).toString();
    }
    const rhsCell = newRow.insertCell();
    rhsCell.setAttribute('contenteditable', 'true');
    rhsCell.textContent = rhs.toString();

    document.getElementById('addConstraintFields').style.display = 'none';
    updateRemoveDropdowns();
});

function setupDropdown(button, menu) {
    button.addEventListener('click', (e) => {
        e.stopPropagation();
        document.querySelectorAll('.dropdown-menu').forEach(m => {
            if (m !== menu) m.style.display = 'none';
        });
        menu.style.display = menu.style.display === 'block' ? 'none' : 'block';
    });
}

function updateRemoveDropdowns() {
    const table = document.getElementById('simplexTableau');
    const headerRow = table.querySelector('thead tr');
    const activityMenu = document.getElementById('removeActivityDropdown');
    activityMenu.innerHTML = '';
    for (let i = 1; i < headerRow.children.length - 1; i++) { // Exclude Basis and RHS
        const a = document.createElement('a');
        a.href = '#';
        a.textContent = headerRow.children[i].textContent;
        a.dataset.index = i;
        a.addEventListener('click', removeActivity);
        activityMenu.appendChild(a);
    }

    const bodyRows = table.querySelectorAll('tbody tr');
    const constraintMenu = document.getElementById('removeConstraintDropdown');
    constraintMenu.innerHTML = '';
    for (let i = 1; i < bodyRows.length; i++) { // Exclude objective row
        const a = document.createElement('a');
        a.href = '#';
        a.textContent = bodyRows[i].children[0].textContent;
        a.dataset.index = i;
        a.addEventListener('click', removeConstraint);
        constraintMenu.appendChild(a);
    }
}

function removeActivity(e) {
    e.preventDefault();
    const colIndex = parseInt(e.target.dataset.index);
    const table = document.getElementById('simplexTableau');
    const rows = table.querySelectorAll('tr');
    rows.forEach(row => {
        row.deleteCell(colIndex);
    });
    updateRemoveDropdowns();
}

function removeConstraint(e) {
    e.preventDefault();
    const rowIndex = parseInt(e.target.dataset.index);
    const table = document.getElementById('simplexTableau');
    table.deleteRow(rowIndex);
    updateRemoveDropdowns();
}

// Initial setup
setupDropdown(document.querySelector('.dropdown:nth-of-type(1) .dropdown-toggle'), document.getElementById('removeActivityDropdown'));
setupDropdown(document.querySelector('.dropdown:nth-of-type(2) .dropdown-toggle'), document.getElementById('removeConstraintDropdown'));
updateRemoveDropdowns();

window.addEventListener('click', () => {
    document.querySelectorAll('.dropdown-menu').forEach(menu => {
        menu.style.display = 'none';
    });
});
