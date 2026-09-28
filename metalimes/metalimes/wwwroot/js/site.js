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

    // Populate Edit Player modal fields (supports different modal/input naming patterns)
    const editPlayerModals = document.querySelectorAll('[id^="editPlayerModal"]');
    editPlayerModals.forEach(function (modal) {
        modal.addEventListener('show.bs.modal', function (event) {
            const button = event.relatedTarget;
            if (!button) return;

            const findField = (selectors) => {
                for (const selector of selectors) {
                    const el = modal.querySelector(selector) || document.querySelector(selector);
                    if (el) return el;
                }
                return null;
            };

            const setField = (selectors, value) => {
                if (value === null || value === undefined) return;
                const el = findField(selectors);
                if (el) el.value = value;
            };

            setField(['#EditPlayer_Id', '[name="EditPlayer.Id"]', '#Id', '[name="Id"]'], button.getAttribute('data-player-id'));
            setField(['#EditPlayer_FirstName', '[name="EditPlayer.FirstName"]', '#FirstName', '[name="FirstName"]'], button.getAttribute('data-first-name'));
            setField(['#EditPlayer_LastName', '[name="EditPlayer.LastName"]', '#LastName', '[name="LastName"]'], button.getAttribute('data-last-name'));
            setField(['#EditPlayer_Email', '[name="EditPlayer.Email"]', '#Email', '[name="Email"]'], button.getAttribute('data-email'));
            setField(['#EditPlayer_FideId', '[name="EditPlayer.FideId"]', '#FideId', '[name="FideId"]'], button.getAttribute('data-fide-id'));
            setField(['#EditPlayer_Rating', '[name="EditPlayer.Rating"]', '#Rating', '[name="Rating"]'], button.getAttribute('data-rating'));

            const statusSelect = findField(['#EditPlayer_Status', '[name="EditPlayer.Status"]', '#Status', '[name="Status"]']);
            if (!statusSelect) return;

            let statusValue = button.getAttribute('data-status');
            if (!statusValue) {
                const row = button.closest('tr');
                const statusCellText = row && row.children && row.children[5] ? row.children[5].textContent : '';
                statusValue = (statusCellText || '').trim();
            }

            // 1) direct value (e.g. "0", "1", "New", "Confirmed")
            statusSelect.value = statusValue;

            // 2) enum-name -> numeric fallback
            if (statusSelect.value !== statusValue) {
                const normalized = String(statusValue).trim().toLowerCase();
                const enumMap = { new: '0', confirmed: '1', cancelled: '2' };
                if (enumMap[normalized] !== undefined) {
                    statusSelect.value = enumMap[normalized];
                }
            }

            // 3) option text fallback
            if (!statusSelect.value || statusSelect.value === '') {
                const normalized = String(statusValue).trim().toLowerCase();
                const match = Array.from(statusSelect.options).find(o =>
                    o.text.trim().toLowerCase() === normalized
                );
                if (match) {
                    statusSelect.value = match.value;
                }
            }
        });
    });
});
