import React from "react";
import PropTypes from "prop-types";
import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";
import Text from "@docspace/components/text";
import { withTranslation } from "react-i18next";
import ModalDialogContainer from "../ModalDialogContainer";
import toastr from "client/toastr";
// import Link from "@docspace/components/link";
// import styled from "styled-components";

class BackupCodesDialogComponent extends React.Component {
  constructor(props) {
    super(props);
  }

  getNewBackupCodes = async () => {
    const { getNewBackupCodes, setBackupCodes } = this.props;
    try {
      const newCodes = await getNewBackupCodes();
      setBackupCodes(newCodes);
    } catch (e) {
      toastr.error(e);
    }
  };

  printPage = () => {
    const { t } = this.props;
    const printContent = document.getElementById("backup-codes-print-content");
    const printWindow = window.open(
      "about:blank",
      "",
      "toolbar=0,scrollbars=1,status=0"
    );
    printWindow.document.write(`<h1>${t("BackupCodesTitle")}</h1>`);
    printWindow.document.write(printContent.innerHTML);
    printWindow.document.close();
    printWindow.focus();
    printWindow.print();
    printWindow.close();
  };

  render() {
    //console.log("Render BackupCodesDialog");
    const {
      t,
      tReady,
      visible,
      onClose,
      backupCodes,
      backupCodesCount,
    } = this.props;

    return (
      <ModalDialogContainer
        isLoading={!tReady}
        visible={visible}
        onClose={onClose}
        autoMaxHeight
        isLarge
      >
        <ModalDialog.Header>{t("BackupCodesTitle")}</ModalDialog.Header>
        <ModalDialog.Body>
          <div id="backup-codes-print-content">
            <Text className="backup-codes-description-one">
              {t("BackupCodesDescription")}
            </Text>
            <Text className="backup-codes-description-two">
              {t("BackupCodesSecondDescription")}
            </Text>

            <Text className="backup-codes-counter" isBold={true}>
              {backupCodesCount} {t("CodesCounter")}
            </Text>

            <Text className="backup-codes-codes" isBold={true}>
              {backupCodes.length > 0 &&
                backupCodes.map((item) => {
                  if (!item.isUsed) {
                    return (
                      <strong key={item.code}>
                        {item.code} <br />
                      </strong>
                    );
                  }
                })}
            </Text>
          </div>
        </ModalDialog.Body>
        <ModalDialog.Footer>
          <Button
            key="RequestNewBtn"
            label={t("RequestNewButton")}
            size="normal"
            primary
            onClick={this.getNewBackupCodes}
          />
          <Button
            key="PrintBtn"
            label={t("Common:CancelButton")}
            size="normal"
            onClick={this.props.onClose}
          />
          {/* <div className="backup-codes-print-link-wrapper">
            <Link type="action" fontSize="13px" isBold={true} isHovered={true}>
              {t("PrintButton")}
            </Link>
          </div> */}
        </ModalDialog.Footer>
      </ModalDialogContainer>
    );
  }
}

const BackupCodesDialog = withTranslation(
  "BackupCodesDialog",
  "Common"
)(BackupCodesDialogComponent);

BackupCodesDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  getNewBackupCodes: PropTypes.func.isRequired,
  backupCodes: PropTypes.array.isRequired,
  backupCodesCount: PropTypes.number.isRequired,
  setBackupCodes: PropTypes.func.isRequired,
};

export default BackupCodesDialog;
