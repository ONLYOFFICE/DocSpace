import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { useTranslation } from "react-i18next";
import Button from "@docspace/components/button";
import Text from "@docspace/components/text";
import ModalDialog from "@docspace/components/modal-dialog";
import { inject, observer } from "mobx-react";

const ModalDialogContainer = styled(ModalDialog)`
  .allowed-number-managers,
  .current-number-managers_number,
  .current-characteristics {
    margin-left: 3px;
  }
  .cannot-downgrade-plan {
    margin-bottom: 16px;
  }

  .downgrade-plan-wrapper {
    display: flex;
  }
`;

const DowngradePlanDialog = ({ visible, onClose }) => {
  const { t, ready } = useTranslation(["DowngradePlanDialog", "Common"]);

  const onCloseModal = () => {
    onClose && onClose();
  };

  return (
    <ModalDialogContainer
      visible={visible}
      onClose={onCloseModal}
      autoMaxHeight
      isLarge
      isLoading={!ready}
    >
      <ModalDialog.Header>
        <Text isBold fontSize="21px">
          {t("DowngradePlan")}
        </Text>
      </ModalDialog.Header>
      <ModalDialog.Body>
        <Text fontSize="13px" isBold className="cannot-downgrade-plan">
          {t("CannotDowngradePlan")} {":"}
        </Text>

        <Text as="span" fontSize="13px">
          {t("AllowedManagerNumber")}
          {":"}
          <Text as="span" isBold className="allowed-number-managers">
            {"1 - 50"}
          </Text>
          {"."}
        </Text>
        <Text as="span" fontSize="13px" className="current-characteristics">
          {t("CurrentManagerNumber")}
          {":"}
          <Text as="span" isBold className="current-number-managers_number">
            {"56"}
          </Text>
          {"."}
        </Text>
        <br />
        {/* TODO: Add converting size */}
        <Text as="span" fontSize="13px">
          {t("StorageSpaceSizeAllowed")}
          {":"}
          <Text as="span" isBold className="current-number-managers_number">
            {"4000"}
          </Text>
          {"."}
        </Text>
        <Text as="span" fontSize="13px" className="current-characteristics">
          {t("CurrentStorageSpace")}
          {":"}
          <Text as="span" isBold className="current-number-managers_number">
            {"4350"}
          </Text>
          {"."}
        </Text>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          label={t("Common:OKButton")}
          size="normal"
          primary={true}
          onClick={onCloseModal}
          tabIndex={3}
        />
        <Button
          label={t("Common:CancelButton")}
          size="normal"
          onClick={onCloseModal}
          tabIndex={3}
        />
      </ModalDialog.Footer>
    </ModalDialogContainer>
  );
};

DowngradePlanDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
};

export default inject(({ payments }) => {
  const {} = payments;

  return {};
})(observer(DowngradePlanDialog));
