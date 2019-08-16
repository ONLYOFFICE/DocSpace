import React, { useCallback } from 'react'
import { withRouter } from 'react-router'
import { Field, reduxForm } from 'redux-form'
import { Avatar, Button, TextInput, Textarea, Label } from 'asc-web-components'
import submit from './submit'
import validate from './validate'
import styled from 'styled-components';


const getUserRole = user => {
  if(user.isOwner) return "owner";
  if(user.isAdmin) return "admin";
  if(user.isVisitor) return "guest";
  return "user";
};

const onEditAvatar = () => {};

const size = {
  mobile: "375px",
  tablet: "768px",
  desktop: "1024px"
};

const device = {
  mobile: `(max-width: ${size.mobile})`,
  tablet: `(max-width: ${size.tablet})`,
  desktop: `(max-width: ${size.desktop})`
};

const MainContainer = styled.div`
  display: flex;
  flex-direction: row;

  @media ${device.tablet} {
    flex-direction: column;
  }
`;

const AvatarContainer = styled.div`
  margin: 0 32px 32px 0;
  width: 160px;
`;

const MainFieldsContainer = styled.div`
  flex-grow: 1;
`;

const FieldContainer = styled.div`
  display: flex;
  flex-direction: row;
  margin: 0 0 16px 0;

  .label {
    line-height: 32px;
    margin: 0;
    width: 110px;
  }

  @media ${device.tablet} {
    flex-direction: column;
    align-items: start;

    .label {
      line-height: unset;
      margin: 0 0 4px 0;
      width: auto;
      flex-grow: 1;
    }
  }
`;



const FieldBody = styled.div`
  flex-grow: 1;
`;

const renderField = ({ input, label, type, meta: { touched, error } }) => (
  <FieldContainer>
    <Label isRequired={true} error={touched && error} text={label} className="label"/>
    <FieldBody>
      <TextInput {...input} type={type} />
      {/* {touched && error && <span>{error}</span>} */}
    </FieldBody>
  </FieldContainer>
)

const UserForm = props => {
  const { error, handleSubmit, submitting, initialValues, userType, history } = props;

  const onCancel = useCallback(() => {
    history.goBack();
  }, [history]);

  return (
    <form onSubmit={handleSubmit(submit)}>
      <MainContainer>
        <AvatarContainer>
          {
            initialValues
            ? <Avatar
                size="max"
                role={getUserRole(initialValues)}
                source={initialValues.avatarMax}
                userName={initialValues.displayName}
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
        </AvatarContainer>
        <MainFieldsContainer>
          <Field
            name="firstName"
            type="text"
            component={renderField}
            label="First name:"
          />
          <Field
            name="lastName"
            type="text"
            component={renderField}
            label="Last name:"
          />
          <Field
            name="email"
            type="text"
            component={renderField}
            label="E-mail:"
          />
        </MainFieldsContainer>
      </MainContainer>
      <div>
        <Label text="Comment"/>
        <Textarea />
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
  form: 'userForm',
  enableReinitialize: true
})(withRouter(UserForm))
