import React from "react";
import PropTypes from "prop-types";
import ModalDialog from "@appserver/components/modal-dialog";
import Button from "@appserver/components/button";
import Text from "@appserver/components/text";
import { Trans, withTranslation } from "react-i18next";
import ModalDialogContainer from "../ModalDialogContainer";

class BackupCodesDialogComponent extends React.Component {
  constructor(props) {
    super(props);
  }

  render() {
    console.log("Render BackupCodesDialog");
    const { t, visible, onClose } = this.props;
    const count = 5; //TODO: get count from api

    return (
      <ModalDialogContainer>
        <ModalDialog visible={visible} onClose={onClose}>
          <ModalDialog.Header>{t("BackupCodesTitle")}</ModalDialog.Header>
          <ModalDialog.Body>
            <Text className="text-dialog">{t("BackupCodesDescription")}</Text>
            <Trans
              t={t}
              i18nKey="BackupCodesCounter"
              ns="BackupCodesDialog"
              count={count}
            >
              <Text>
                Now you have only {{ count }} code(s) each can be used only one
                time. Print them and use when needed.
              </Text>
            </Trans>
            <Text className="text-dialog" textAlign="center" isBold={true}>
              code1 <br />
              code2 <br />
              code3 <br />
              code4 <br />
              code5
            </Text>
            <Text className="text-dialog">
              {t("BackupCodesFooterDescription")}
            </Text>
          </ModalDialog.Body>
          <ModalDialog.Footer>
            <Button
              key="SendBtn"
              label={t("PrintButton")}
              size="medium"
              primary={true}
            />
            <Button
              key="RequestNewBtn"
              className="button-dialog"
              label={t("RequestNewButton")}
              size="medium"
              primary={false}
            />

            <Button
              key="CloseBtn"
              className="button-dialog"
              label={t("CloseButton")}
              size="medium"
              primary={false}
              onClick={onClose}
            />
          </ModalDialog.Footer>
        </ModalDialog>
      </ModalDialogContainer>
    );
  }
}

const BackupCodesDialog = withTranslation("BackupCodesDialog")(
  BackupCodesDialogComponent
);

BackupCodesDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
};

export default BackupCodesDialog;
