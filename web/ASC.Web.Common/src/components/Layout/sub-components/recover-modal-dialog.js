import React from "react";
import PropTypes from "prop-types";
import { Button, TextInput, Text, ModalDialog, Textarea } from "asc-web-components";

const SubModalDialog = ({ visible, onRecoverModalClose, t }) => {

  const onSendRecoverInstructions = () => {
    alert("Send button clicked")
  }
  
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
        <TextInput
          key="e-mail"
          id="e-mail"
          name="e-mail"
          type="text"
          size="base"
          scale={true}
          tabIndex={1}
          style={{ marginTop: "16px", marginBottom: "16px" }}
          placeholder={t("RecoverContactEmailPlaceholder")}
          //isDisabled={isLoading}
          //value={email}
          onChange={event => console.log(event.target.value)}
        />,
        <Textarea
          key="text-description"
          placeholder={t("RecoverDescribeYourProblemPlaceholder")}
          onChange={event => console.log(event.target.value)}
        //value="value"
        />
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
  t: PropTypes.func.isRequired
  // t: PropTypes.func.isRequired
};

export default SubModalDialog;
