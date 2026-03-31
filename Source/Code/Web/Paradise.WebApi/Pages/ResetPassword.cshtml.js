const PasswordValidator = {
  rules: [
    { name: "length", regex: /.{8,}/ },
    { name: "uppercase", regex: /[A-Z]/ },
    { name: "lowercase", regex: /[a-z]/ },
    { name: "number", regex: /[0-9]/ },
    { name: "special", regex: /[^A-Za-z0-9]/ }
  ],

  getPassedRules(password) {
    return this.rules.filter(rule => rule.regex.test(password)).map(rule => rule.name);
  },

  isValid(password) {
    return this.rules.every(rule => rule.regex.test(password));
  }
};

document.addEventListener("DOMContentLoaded", () => {
  const passwordInput = document.getElementById("password");
  const passwordConfirmationInput = document.getElementById("confirm-password");
  const submitButton = document.getElementById("submit-btn");
  const requirements = document.querySelectorAll("#requirements-list li");

  let passwordIsValid = false;

  function getPassedRequirements() {
    const password = passwordInput.value;
    return PasswordValidator.getPassedRules(password);
  }

  function updatePassedRequirements(passedRequirements) {
    requirements.forEach(li => {
      const requirement = li.dataset.requirement;
      li.className = passedRequirements.includes(requirement) ? "valid" : "invalid";
    });
  }

  function updateConfirmationInput(matches, confirmationLength) {
    passwordConfirmationInput.classList.toggle(
      "is-invalid",
      confirmationLength > 0 && !matches
    );
  }

  function validatePassword() {
    const passedRequirements = getPassedRequirements();
    updatePassedRequirements(passedRequirements);

    const password = passwordInput.value;
    passwordIsValid = PasswordValidator.isValid(password);
  }

  function validateConfirmation() {
    const password = passwordInput.value;
    const confirmation = passwordConfirmationInput.value;
    const matches = password.length > 0 && password === confirmation;

    updateConfirmationInput(matches, confirmation.length);

    submitButton.disabled = !(passwordIsValid && matches);
  }

  function validate() {
    validatePassword();
    validateConfirmation();
  }

  passwordInput.addEventListener('input', validate);
  passwordConfirmationInput.addEventListener('input', validateConfirmation);
});