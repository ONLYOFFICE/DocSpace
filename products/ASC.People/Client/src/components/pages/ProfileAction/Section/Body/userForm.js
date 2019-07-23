import React from 'react'
import { Field, reduxForm, SubmissionError } from 'redux-form'
import { createUser, updateUser } from '../../../../../utils/api';


const submit = values => {
    function successCallback (res) {
      if (res.data && res.data.error) {
        window.alert(res.data.error.message);
      } else {
        console.log(res);
        window.alert('Success');
      }
    }

    function errorCallback (error) {
      throw new SubmissionError({
        _error: error
      })
    }

    if (values.id) {
      updateUser(values).then(successCallback).catch(errorCallback);
    } else {
      createUser(values).then(successCallback).catch(errorCallback);
    }
}

const validate = values => {
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

const renderField = ({ input, label, type, meta: { touched, error } }) => (
  <div>
    <label>{label}</label>
    <div>
      <input {...input} placeholder={label} type={type} />
      {touched && error && <span>{error}</span>}
    </div>
  </div>
)

const UserForm = props => {
  const { error, handleSubmit, pristine, reset, submitting } = props
  return (
    <form onSubmit={handleSubmit(submit)}>
      <Field
        name="firstName"
        type="text"
        component={renderField}
        label="First Name"
      />
      <Field
        name="lastName"
        type="text"
        component={renderField}
        label="Last Name"
      />
      <Field
        name="email"
        type="text"
        component={renderField}
        label="Email"
      />
      {error && <strong>{error}</strong>}
      <div>
        <button type="submit" disabled={submitting}>
          Save
        </button>
        <button type="button" disabled={pristine || submitting} onClick={reset}>
          Clear
        </button>
      </div>
    </form>
  )
}

export default reduxForm({
  validate,
  form: 'userForm',
  enableReinitialize: true
})(UserForm)
