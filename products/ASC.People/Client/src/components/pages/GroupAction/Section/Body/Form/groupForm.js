import React, { useCallback } from 'react'
import { withRouter } from 'react-router'
import { Field, reduxForm, SubmissionError } from 'redux-form'
import { Button, TextInput, InputBlock, Icons, SelectedItem } from 'asc-web-components'
import submit from './submit'
import validate from './validate'

const GroupForm = props => {
  const { error, handleSubmit, submitting, initialValues, history } = props;

  const onCancel = useCallback(() => {
    history.goBack();
  }, [history]);

  return (
    <form onSubmit={handleSubmit(submit)}>
      <div style={{display: "flex"}}>
        <div>
          <label>Department name:</label>
          <TextInput name="group-name" />
        </div>
      </div>
      <div style={{marginTop: "16px"}}>
        <div>Head of department:</div>
        <InputBlock placeholder="Add employee"
          iconName='ExpanderDownIcon'
          iconSize={8}
          isIconFill={true}
          iconColor='#A3A9AE'
          scale={false}
          isReadOnly={true}
        >
            <Icons.CatalogEmployeeIcon size="medium" />
        </InputBlock>
      </div>
      <div style={{marginTop: "16px"}}>
        <div>Members:</div>
        <InputBlock placeholder="Add employee"
          iconName='ExpanderDownIcon'
          iconSize={8}
          isIconFill={true}
          iconColor='#A3A9AE'
          scale={false}
          isReadOnly={true}
        >
            <Icons.CatalogEmployeeIcon size="medium" />
        </InputBlock>
      </div>
      <div style={{marginTop: "16px", display: "flex", flexWrap: "wrap;"}}>
        <SelectedItem text="Fake User 1" onClick={()=>console.log("onClose 1")} isInline={true} />
        <SelectedItem text="Fake User 2" onClick={()=>console.log("onClose 2")} isInline={true} />
        <SelectedItem text="Fake User 3" onClick={()=>console.log("onClose 3")} isInline={true} />
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
