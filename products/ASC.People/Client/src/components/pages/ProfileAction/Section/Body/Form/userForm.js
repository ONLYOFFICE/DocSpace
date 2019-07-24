import React from 'react'
import { Field, reduxForm, SubmissionError } from 'redux-form'
import { Avatar, Button } from 'asc-web-components'
import submit from './submit'
import validate from './validate'


const getUserRole = user => {
  if(user.isOwner) return "owner";
  if(user.isAdmin) return "admin";
  if(user.isVisitor) return "guest";
  return "user";
};

const onEditAvatar = () => {};

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
  const { error, handleSubmit, submitting, initialValues, userType } = props
  return (
    <form onSubmit={handleSubmit(submit)}>
      <div style={{display: "flex"}}>
        <div style={{marginRight: "30px"}}>
          {
            initialValues
            ? <Avatar
                size="max"
                role={getUserRole(initialValues)}
                source={initialValues.avatar}
                userName={initialValues.userName}
                editing={true}
                editLabel={"Edit photo"}
                editAction={onEditAvatar}
              />
            : <Avatar
                size="max"
                role={userType}
                editing={true}
                editLabel={"Add photo"}
                editAction={onEditAvatar}
              />
          }
        </div>
        <div>
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
        </div>
      </div>
      <div>
        <div>Comment</div>
        <textarea style={{width: "100%"}}></textarea>
      </div>
      <div>
        <div>Contact Information</div>
        <input type="text"/>
      </div>
      <div>
        <div>Social Profiles</div>
        <input type="text"/>
      </div>
      <div>
        {error && <strong>{error}</strong>}
      </div>
      <div style={{marginTop: "60px"}}>
        <Button label="Save" primary type="submit" isDisabled={submitting} onClick={()=>{}}/>
        <Button label="Cancel" style={{ marginLeft: '8px' }} isDisabled={submitting} onClick={()=>{}}/>
      </div>
    </form>
  )
}

export default reduxForm({
  validate,
  form: 'userForm',
  enableReinitialize: true
})(UserForm)
