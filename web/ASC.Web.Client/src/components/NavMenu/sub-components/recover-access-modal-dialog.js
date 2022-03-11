import React, { useState } from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

import Button from "@appserver/components/button";
import TextInput from "@appserver/components/text-input";
import Text from "@appserver/components/text";
import ModalDialog from "@appserver/components/modal-dialog";
import Textarea from "@appserver/components/textarea";
import FieldContainer from "@appserver/components/field-container";

const ModalDialogContainer = styled(ModalDialog)`
  .modal-dialog-aside-footer {
    @media (max-width: 1024px) {
      width: 90%;
    }
  }

  .recover-button-dialog {
    @media (max-width: 1024px) {
      width: 100%;
    }
  }

  .text-body {
    margin-bottom: 16px;
  }
`;

const RecoverAccessModalDialog = ({
  visible,
  loading,
  email,
  emailErr,
  description,
  descErr,
  t,
  onChangeEmail,
  onChangeDescription,
  onRecoverModalClose,
  onSendRecoverRequest,
}) => {
  const [width, setWidth] = useState(window.innerWidth);

  React.useEffect(() => {
    window.addEventListener("resize", () => setWidth(window.innerWidth));
  }, []);

  return (
    <ModalDialogContainer
      visible={visible}
      modalBodyPadding="12px 0 0 0"
      asideBodyPadding="16px 0 0 0"
      onClose={onRecoverModalClose}
    >
      <ModalDialog.Header>
        <Text isBold={true} fontSize="21px">
          {t("RecoverTitle")}
        </Text>
      </ModalDialog.Header>
      <ModalDialog.Body>
        <Text
          key="text-body"
          className="text-body"
          isBold={false}
          fontSize="13px"
        >
          {t("RecoverTextBody")}
        </Text>
        <FieldContainer
          key="e-mail"
          isVertical={true}
          labelVisible={false}
          hasError={emailErr}
          errorMessage={t("Common:RequiredField")}
        >
          <TextInput
            hasError={emailErr}
            id="e-mail"
            name="e-mail"
            type="text"
            size="base"
            scale={true}
            tabIndex={3}
            placeholder={t("RecoverContactEmailPlaceholder")}
            isAutoFocussed={true}
            isDisabled={loading}
            value={email}
            onChange={onChangeEmail}
          />
        </FieldContainer>
        <FieldContainer
          key="text-description"
          isVertical={true}
          hasError={descErr}
          labelVisible={false}
          errorMessage={t("Common:RequiredField")}
        >
          <Textarea
            heightScale={width > 1024 ? false : true}
            hasError={descErr}
            placeholder={t("RecoverDescribeYourProblemPlaceholder")}
            tabIndex={3}
            value={description}
            onChange={onChangeDescription}
            isDisabled={loading}
          />
        </FieldContainer>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          className="recover-button-dialog"
          key="SendBtn"
          label={loading ? t("Common:Sending") : t("Common:SendButton")}
          size="normal"
          primary={true}
          onClick={onSendRecoverRequest}
          isLoading={loading}
          isDisabled={loading}
          tabIndex={3}
        />
      </ModalDialog.Footer>
    </ModalDialogContainer>
  );
};

RecoverAccessModalDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  loading: PropTypes.bool.isRequired,
  email: PropTypes.string,
  emailErr: PropTypes.bool.isRequired,
  description: PropTypes.string,
  descErr: PropTypes.bool.isRequired,
  t: PropTypes.func.isRequired,
  onChangeEmail: PropTypes.func.isRequired,
  onChangeDescription: PropTypes.func.isRequired,
  onRecoverModalClose: PropTypes.func.isRequired,
  onSendRecoverRequest: PropTypes.func.isRequired,
};

export default RecoverAccessModalDialog;
