// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Function to set edit user data in modal
function setEditUser(userId, username) {
    // Set Edit User ID
    document.getElementById('editEditUserId').value = userId;

    // Set username
    document.getElementById('editNewUsername').value = username;

    // Clear password field
    document.getElementById('editNewPassword').value = '';

    // Remove 'required' attribute from password field when editing (it's optional)
    const passwordInput = document.getElementById('editNewPassword');
    if (passwordInput) {
        passwordInput.removeAttribute('required');
    }

    // Get the table row with user data
    const userRow = document.querySelector(`table tr[data-roles]`);
    if (userRow) {
        const rolesAttr = userRow.getAttribute('data-roles');
        const selectedRoleIds = rolesAttr ? rolesAttr.split(',').filter(r => r.trim() !== '') : [];

        // Uncheck all role checkboxes first
        const roleCheckboxes = document.querySelectorAll('input[name="SelectedRoles"]');
        roleCheckboxes.forEach(checkbox => {
            checkbox.checked = false;
        });

        // Check the appropriate checkboxes based on selected roles
        selectedRoleIds.forEach(roleId => {
            const checkbox = document.querySelector(`input[name="SelectedRoles"][value="${roleId}"]`);
            if (checkbox) {
                checkbox.checked = true;
            }
        });
    }
}

// Function to handle role update form submission
function handleRoleCheckboxes() {
    const form = document.querySelector('form');
    form.addEventListener('change', function(e) {
        if (e.target.name === 'SelectedRoles') {
            // The checkboxes will automatically bind to the SelectedRoles list
            console.log('Role checkbox changed');
        }
    });
}

// Handle password field validation based on modal type
document.addEventListener('DOMContentLoaded', function() {
    const createUserModal = document.getElementById('createUserModal');
    const editUserModal = document.getElementById('editUserModal');
    const passwordInput = document.getElementById('editNewPassword');
    const createPasswordInput = document.getElementById('newPassword'); // Adjust as needed

    if (createUserModal) {
        createUserModal.addEventListener('shown.bs.modal', function() {
            // For create modal, password is required
            if (createPasswordInput) {
                createPasswordInput.setAttribute('required', 'required');
            }
        });
    }

    if (editUserModal) {
        editUserModal.addEventListener('shown.bs.modal', function() {
            // For edit modal, password is optional
            if (passwordInput) {
                passwordInput.removeAttribute('required');
            }
        });
    }
});
