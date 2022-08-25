import React, { useState } from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

import Button from "@docspace/components/button";
import TextInput from "@docspace/components/text-input";
import Text from "@docspace/components/text";
import ModalDialog from "@docspace/components/modal-dialog";
import Textarea from "@docspace/components/textarea";
import FieldContainer from "@docspace/components/field-container";
import { smallTablet } from "@docspace/components/utils/device";
import { sendRecoverRequest } from "@docspace/common/api/settings";
import toastr from "@docspace/components/toast/toastr";

const ModalDialogContainer = styled(ModalDialog)`
  .modal-dialog-aside-footer {
    @media (max-width: 1024px) {
      width: 90%;
    }
  }

  .recover-button-dialog {
    @media ${smallTablet} {
      width: 100%;
    }
  }

  .text-body {
    margin-bottom: 16px;
  }

  .textarea {
    margin-bottom: 0;
  }
`;

const RecoverAccessModalDialog = ({ t, visible, onClose }) => {
  const [loading, setLoading] = useState(false);

  const [email, setEmail] = useState("");
  const [emailErr, setEmailErr] = useState(false);

  const [description, setDescription] = useState("");
  const [descErr, setDescErr] = useState(false);

  const onRecoverModalClose = () => {
    setEmail("");
    setEmailErr(false);
    setDescription("");
    setDescErr(false);
    onClose && onClose();
  };

  const onChangeEmail = (e) => {
    setEmail(e.currentTarget.value);
    setEmailErr(false);
  };

  const onChangeDescription = (e) => {
    setDescription(e.currentTarget.value);
    setDescErr(false);
  };

  const onSendRecoverRequest = () => {
    if (!email.trim()) {
      setEmailErr(true);
    }
    if (!description.trim()) {
      setDescErr(true);
    } else {
      setLoading(true);
      sendRecoverRequest(email, description)
        .then((res) => {
          setLoading(false);
          toastr.success(res);
        })
        .catch((error) => {
          setLoading(false);
          toastr.error(error);
        })
        .finally(onRecoverModalClose);
    }
  };

  return (
    <ModalDialogContainer
      visible={visible}
      onClose={onRecoverModalClose}
      isLarge
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
          className="textarea"
          key="text-description"
          isVertical={true}
          hasError={descErr}
          labelVisible={false}
          errorMessage={t("Common:RequiredField")}
        >
          <Textarea
            heightScale={false}
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
        <Button
          className="recover-button-dialog"
          key="SendBtn"
          label={t("Common:CancelButton")}
          size="normal"
          onClick={onRecoverModalClose}
          isLoading={loading}
          isDisabled={loading}
          tabIndex={3}
        />
      </ModalDialog.Footer>
    </ModalDialogContainer>
  );
};

RecoverAccessModalDialog.propTypes = {
  t: PropTypes.func.isRequired,
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
};

export default RecoverAccessModalDialog;
