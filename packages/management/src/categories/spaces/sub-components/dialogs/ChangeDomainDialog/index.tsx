import React, { useEffect } from "react";
import styled from "styled-components";
import ModalDialogContainer from "@docspace/client/src/components/dialogs/ModalDialogContainer";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import ModalDialog from "@docspace/components/modal-dialog";
import { useTranslation } from "react-i18next";
import { observer } from "mobx-react";
import { TextInput, Checkbox } from "@docspace/components";
import { parseAddress } from "@docspace/components/utils/email";
import { useStore } from "SRC_DIR/store";

const StyledModal = styled(ModalDialogContainer)`
  .create-docspace-input-block {
    padding-top: 16px;
  }
  .create-docspace-input {
    width: 100%;
  }
`;

const ChangeDomainDialogComponent = () => {
  const { t } = useTranslation(["Management", "Common"]);
  const { spacesStore } = useStore();

  const {
    setPortalSettings,
    getAllPortals,
    getPortalDomain,
    setChangeDomainDialogVisible,
    domainDialogVisible: visible,
  } = spacesStore;

  const [domain, setDomain] = React.useState("");

  const onHandleDomain = (e) => {
    setDomain(e.target.value);
  };

  const onClose = () => {
    setChangeDomainDialogVisible(false);
  };

  const onClickDomainChange = async () => {
    await setPortalSettings(domain);
    await getAllPortals();
    await getPortalDomain();
    onClose();
  };

  let parsed = parseAddress("test@" + domain);
  const isDomainError = domain.length > 0 && !parsed.isValid();
  return (
    <StyledModal
      visible={visible}
      isLarge
      onClose={onClose}
      displayType="modal"
    >
      <ModalDialog.Header>{t("DomainSettings")}</ModalDialog.Header>
      <ModalDialog.Body>
        <Text noSelect={true} fontSize="13px">
          {t("ChangeDomainDescription")}
        </Text>
        <div className="create-docspace-input-block">
          <Text
            color="#333"
            fontSize="13px"
            fontWeight="600"
            style={{ paddingBottom: "5px" }}
          >
            {t("DomainName")}
          </Text>
          <TextInput
            hasError={isDomainError}
            onChange={onHandleDomain}
            value={domain}
            placeholder={t("EnterDomain")}
            className="create-docspace-input"
          />
        </div>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          key="CreateButton"
          label={t("Common:ChangeButton")}
          isDisabled={isDomainError || domain.length === 0}
          onClick={onClickDomainChange}
          size="normal"
          primary
          scale={true}
        />
        <Button
          key="CancelButton"
          label={t("Common:CancelButton")}
          size="normal"
          onClick={onClose}
          scale={true}
        />
      </ModalDialog.Footer>
    </StyledModal>
  );
};

export default observer(ChangeDomainDialogComponent);
