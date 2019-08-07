import React, { useCallback } from 'react'
import { withRouter } from 'react-router'
import { Field, reduxForm, SubmissionError } from 'redux-form'
import { Button } from 'asc-web-components'
import submit from './submit'
import validate from './validate'

const renderField = ({ input, label, type, meta: { touched, error } }) => (
  <div>
    <label>{label}</label>
    <div>
      <input {...input} placeholder={label} type={type} />
      {touched && error && <span>{error}</span>}
    </div>
  </div>
)

const GroupForm = props => {
  const { error, handleSubmit, submitting, initialValues, history } = props;

  const onCancel = useCallback(() => {
    history.goBack();
  }, [history]);

  return (
    <form onSubmit={handleSubmit(submit)}>
      <div style={{display: "flex"}}>
        <div>
          <Field
            name="groupName"
            type="text"
            component={renderField}
            label="Department name"
          />
        </div>
      </div>
      <div>
        <div>Head of department:</div>
      </div>
      <div>
        <div>Members:</div>
      </div>
      <div>
        {error && <strong>{error}</strong>}
      </div>
      <div style={{marginTop: "60px"}}>
        <Button label="Save" primary type="submit" isDisabled={submitting}/>
        <Button label="Cancel" style={{ marginLeft: '8px' }} isDisabled={submitting} onClick={onCancel}/>
      </div>
    </form>
  )
}

export default reduxForm({
  validate,
  form: 'groupForm',
  enableReinitialize: true
})(withRouter(GroupForm))
