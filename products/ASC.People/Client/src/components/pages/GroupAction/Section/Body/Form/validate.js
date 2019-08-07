function validate (values) {
    const errors = {};

    if (!values.firstName) {
      errors.firstName = 'required field';
    }

    if (!values.lastName) {
      errors.lastName = 'required field';
    }

    if (!values.email) {
      errors.email = 'required field';
    }

    return errors
};

export default validate;