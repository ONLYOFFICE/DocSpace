function validate (values) {
    const errors = {};

    if (!values.firstName) {
      errors.firstName = 'first name is required field';
    }

    if (!values.lastName) {
      errors.lastName = 'last name is required field';
    }

    if (!values.email) {
      errors.email = 'email is required field';
    }

    if (!values.password) {
      errors.password = 'password is required field';
    }

    return errors
};

export default validate;