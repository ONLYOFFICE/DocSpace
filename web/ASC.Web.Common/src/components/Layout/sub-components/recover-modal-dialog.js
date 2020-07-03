import React from "react";
import PropTypes from "prop-types";
import { Button, TextInput, Text, ModalDialog } from "asc-web-components";

const SubModalDialog = ({visible, onRecoverModalClose}) => {

    // const {
    //   visible,
    //   isLoading,
    //   email,
    //   onChangeEmail,
    //   onSendPasswordInstructions,
    //   onRecoverModalClose,
    //   t
    // } = this.props;
    return (
      <ModalDialog
        visible={visible}
        headerContent={
          <Text isBold={true} fontSize='21px'>
            Access recovery
          </Text>
        }
        bodyContent={[
          <Text
            key="text-body"
            className="text-body"
            isBold={false}
            fontSize='13px'
          >
            If you are an existing user and have problems accessing this portal or you want to register as the portal new user, please contact the portal administrator using the form below.
          </Text>,
          <TextInput
            key="e-mail"
            id="e-mail"
            name="e-mail"
            type="text"
            size="base"
            scale={true}
            tabIndex={1}
            style={{marginTop: "16px"}}
            //isDisabled={isLoading}
            //value={email}
            //onChange={onChangeEmail}
          />
        ]}
        footerContent={[
          <Button
            className="login-button-dialog"
            key="SendBtn"
            //label={isLoading ? "t" : "s"}
            size="big"
            scale={false}
            primary={true}
            //onClick={onSendPasswordInstructions}
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
  // t: PropTypes.func.isRequired
};

export default SubModalDialog;
