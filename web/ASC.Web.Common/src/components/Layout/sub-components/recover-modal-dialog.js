import React, { useState } from "react";
import PropTypes from "prop-types";
import { Button, TextInput, Text, ModalDialog, Textarea, FieldContainer } from "asc-web-components";

const SubModalDialog = ({ visible, onRecoverModalClose, t }) => {

  const [email, setEmail] = useState("");
  const [description, setDescription] = useState("");
  const [err, setErr] = useState(false);

  const onSendRecoverInstructions = () => {
    if (!email.trim()) {
      setErr(true);
    }
    else if (!description.trim()) {
      setErr(true);
    }
    else {
      console.log(`Access recovery sent. 
      E-mail: ${email}, 
      Description: ${description}`);
    }
  };

  const onChangeEmail = (e) => setEmail(e.currentTarget.value);

  const onChangeDescription = (e) => setDescription(e.currentTarget.value);

  return (
    <ModalDialog
      visible={visible}
      headerContent={
        <Text isBold={true} fontSize='21px'>
          {t("RecoverTitle")}
        </Text>
      }
      bodyContent={[
        <Text
          key="text-body"
          className="text-body"
          isBold={false}
          fontSize='13px'
        >
          {t("RecoverTextBody")}
        </Text>,
        <FieldContainer key="e-mail" isVertical={true} labelText={"e-mail"}>
          <TextInput
            hasError={err}
            id="e-mail"
            name="e-mail"
            type="text"
            size="base"
            scale={true}
            tabIndex={1}
            style={{ marginTop: "16px", marginBottom: "16px" }}
            placeholder={t("RecoverContactEmailPlaceholder")}
            //isDisabled={isLoading}
            value={email}
            onChange={onChangeEmail}
          />
        </FieldContainer>,
        <FieldContainer key="text-description" isVertical={true}>
          <Textarea
            hasError={err}
            placeholder={t("RecoverDescribeYourProblemPlaceholder")}
            value={description}
            onChange={onChangeDescription}
          />
        </FieldContainer>
      ]}
      footerContent={[
        <Button
          className="login-button-dialog"
          key="SendBtn"
          label={t("RecoverSendButton")}
          size="big"
          scale={false}
          primary={true}
          onClick={onSendRecoverInstructions}
          //isLoading={isLoading}
          //isDisabled={isLoading}
          tabIndex={2}
        />
      ]}
      onClose={onRecoverModalClose}
    />
  );
}

SubModalDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  // isLoading: PropTypes.bool.isRequired,
  // email: PropTypes.string.isRequired,
  // onChangeEmail: PropTypes.func.isRequired,
  // onSendPasswordInstructions: PropTypes.func.isRequired,
  onRecoverModalClose: PropTypes.func.isRequired,
  onChangeEmail: PropTypes.func.isRequired,
  onChangeDescription: PropTypes.func.isRequired,
  t: PropTypes.func.isRequired
  // t: PropTypes.func.isRequired
};

export default SubModalDialog;
