import React, { useState, useEffect } from "react";
import PropTypes from "prop-types";
import { Button, TextInput, Text, ModalDialog, Textarea, FieldContainer } from "asc-web-components";
import styled from "styled-components";

const ModalDialogContainer = styled.div`
  .modal-dialog-aside-footer {

    @media(max-width: 1024px) {
      width: 90%;
    }
  }

  .recover-button-dialog {

    @media(max-width: 1024px) {
      width: 100%;
    }
  }

  .text-body {
    margin-bottom: 16px;
  }
`;

const SubModalDialog = ({ visible, onRecoverModalClose, t }) => {

  const [email, setEmail] = useState("");
  const [emailErr, setEmailErr] = useState(false);
  const [description, setDescription] = useState("");
  const [descErr, setDescErr] = useState(false);
  const [width, setWidth] = useState(window.innerWidth);

  const onSendRecoverInstructions = () => {
    if (!email.trim()) {
      setEmailErr(true);
    }
    if (!description.trim()) {
      setDescErr(true);
    }
    else {
      console.log(`Access recovery sent. 
      E-mail: ${email}, 
      Description: ${description}`);
    }
  };

  const onChangeEmail = (e) => setEmail(e.currentTarget.value);

  const onChangeDescription = (e) => setDescription(e.currentTarget.value);

  useEffect(() => {
    window.addEventListener("resize", () => setWidth(window.innerWidth));
  }, []);

  return (
    <ModalDialogContainer>
      <ModalDialog
        visible={visible}
        bodyPadding="16px 0 0 0"
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
          <FieldContainer
            key="e-mail"
            isVertical={true}
            labelVisible={false}
            hasError={emailErr}
            errorMessage={t("RecoverErrorMessage")}>
            <TextInput
              hasError={emailErr}
              id="e-mail"
              name="e-mail"
              type="text"
              size="base"
              scale={true}
              tabIndex={1}
              placeholder={t("RecoverContactEmailPlaceholder")}
              isAutoFocussed={true}
              //isDisabled={isLoading}
              value={email}
              onChange={onChangeEmail}
            />
          </FieldContainer>,
          <FieldContainer
            key="text-description"
            isVertical={true}
            hasError={descErr}
            labelVisible={false}
            errorMessage={t("RecoverErrorMessage")}>
            <Textarea
              heightScale={width > 1024 ? false : true}
              hasError={descErr}
              placeholder={t("RecoverDescribeYourProblemPlaceholder")}
              tabIndex={1}
              value={description}
              onChange={onChangeDescription}
              //isDisabled={isLoading}
            />
          </FieldContainer>
        ]}
        footerContent={[
          <Button
            className="recover-button-dialog"
            key="SendBtn"
            label={t("RecoverSendButton")}
            size="big"
            primary={true}
            onClick={onSendRecoverInstructions}
            //isLoading={isLoading}
            //isDisabled={isLoading}
            tabIndex={2}
          />
        ]}
        onClose={onRecoverModalClose}
      />
    </ModalDialogContainer>
  );
}

SubModalDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  // isLoading: PropTypes.bool.isRequired,
  onSendRecoverInstructions: PropTypes.func,
  onRecoverModalClose: PropTypes.func.isRequired,
  onChangeEmail: PropTypes.func.isRequired,
  onChangeDescription: PropTypes.func.isRequired,
  t: PropTypes.func.isRequired
};

export default SubModalDialog;
