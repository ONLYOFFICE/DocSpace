import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import ModalDialog from "@docspace/components/modal-dialog";
import EmailInput from "@docspace/components/email-input";
import Button from "@docspace/components/button";
import Box from "@docspace/components/box";
import Text from "@docspace/components/text";
import { tablet } from "@docspace/components/utils/device";

const BtnContainer = styled(Box)`
  width: 100px;

  @media ${tablet} {
    width: 293px;
  }
`;

const BodyContainer = styled(Box)`
  font: 13px "Open Sans", normal;
  line-height: 20px;
`;

const ModalContainer = ({
  t,
  errorLoading,
  visibleModal,
  errorMessage,
  emailOwner,
  settings,
  onEmailChangeHandler,
  onSaveEmailHandler,
  onCloseModal,
  checkingMessages,
}) => {
  let header, content, footer;

  const visible = errorLoading ? errorLoading : visibleModal;

  if (errorLoading) {
    header = t("ErrorLicenseTitle");
    content = (
      <BodyContainer>
        {errorMessage ? errorMessage : t("ErrorLicenseBody")}
      </BodyContainer>
    );
  } else if (visibleModal && checkingMessages.length < 1) {
    header = t("ChangeEmailTitle");

    content = (
      <EmailInput
        tabIndex={1}
        scale={true}
        size="base"
        id="change-email"
        name="email-wizard"
        placeholder={t("Common:Email")}
        emailSettings={settings}
        value={emailOwner}
        onValidateInput={onEmailChangeHandler}
      />
    );

    footer = (
      <BtnContainer>
        <Button
          key="saveBtn"
          label={t("Common:SaveButton")}
          primary={true}
          scale={true}
          size="normal"
          onClick={onSaveEmailHandler}
        />
      </BtnContainer>
    );
  } else if (visibleModal && checkingMessages.length > 0) {
    header = t("ErrorParamsTitle");

    content = (
      <>
        <Text as="p">{t("ErrorParamsBody")}</Text>
        {checkingMessages.map((el, index) => (
          <Text key={index} as="p">
            - {el};
          </Text>
        ))}
      </>
    );

    footer = (
      <BtnContainer>
        <Button
          key="saveBtn"
          label={t("Common:CloseButton")}
          primary={true}
          scale={true}
          size="normal"
          onClick={onCloseModal}
        />
      </BtnContainer>
    );
  }

  return (
    <ModalDialog
      visible={visible}
      displayType="modal"
      zIndex={310}
      onClose={onCloseModal}
    >
      <ModalDialog.Header>{header}</ModalDialog.Header>
      <ModalDialog.Body>{content}</ModalDialog.Body>
      <ModalDialog.Footer>{footer}</ModalDialog.Footer>
    </ModalDialog>
  );
};

ModalContainer.propTypes = {
  t: PropTypes.func.isRequired,
  errorLoading: PropTypes.bool.isRequired,
  visibleModal: PropTypes.bool.isRequired,
  emailOwner: PropTypes.string,
  settings: PropTypes.object.isRequired,
  onEmailChangeHandler: PropTypes.func.isRequired,
  onSaveEmailHandler: PropTypes.func.isRequired,
  onCloseModal: PropTypes.func.isRequired,
};

export default ModalContainer;
