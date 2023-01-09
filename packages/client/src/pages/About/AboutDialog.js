import React from "react";
import PropTypes from "prop-types";
import ModalDialog from "@docspace/components/modal-dialog";
import ModalDialogContainer from "./ModalDialogContainer";
import { I18nextProvider, useTranslation } from "react-i18next";
import AboutContent from "./AboutContent";
import i18n from "./i18n";

const AboutDialog = (props) => {
  const { visible, onClose, personal, buildVersionInfo, previewData } = props;
  const { t, ready } = useTranslation(["About", "Common"]);

  return (
    <ModalDialogContainer
      isLoading={!ready}
      visible={visible}
      onClose={onClose}
      displayType="modal"
      isLarge
    >
      <ModalDialog.Header>{t("AboutHeader")}</ModalDialog.Header>
      <ModalDialog.Body>
        <AboutContent
          personal={personal}
          buildVersionInfo={buildVersionInfo}
          previewData={previewData}
        />
      </ModalDialog.Body>
    </ModalDialogContainer>
  );
};

AboutDialog.propTypes = {
  visible: PropTypes.bool,
  onClose: PropTypes.func,
  personal: PropTypes.bool,
  buildVersionInfo: PropTypes.object,
  previewData: PropTypes.object,
};

const AboutDialogWrapper = (props) => {
  return (
    <I18nextProvider i18n={i18n}>
      <AboutDialog {...props} />
    </I18nextProvider>
  );
};

export default AboutDialogWrapper;
