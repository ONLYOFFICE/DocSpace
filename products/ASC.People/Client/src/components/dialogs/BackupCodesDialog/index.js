import React from "react";
import PropTypes from "prop-types";
import ModalDialog from "@appserver/components/modal-dialog";
import Button from "@appserver/components/button";
import Text from "@appserver/components/text";
import { withTranslation } from "react-i18next";
import ModalDialogContainer from "../ModalDialogContainer";
import toastr from "studio/toastr";

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
      >
        <ModalDialog.Header>{t("BackupCodesTitle")}</ModalDialog.Header>
        <ModalDialog.Body>
          <div id="backup-codes-print-content">
            <Text className="text-dialog">{t("BackupCodesDescription")}</Text>
            <Text className="text-dialog">
              {t("BackupCodesSecondDescription")}
            </Text>

            <Text className="text-dialog" isBold={true}>
              {backupCodesCount} {t("CodesCounter")}
            </Text>

            <Text className="text-dialog" isBold={true}>
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
            key="PrintBtn"
            label={t("PrintButton")}
            size="small"
            primary={true}
            onClick={this.printPage}
          />
          <Button
            key="RequestNewBtn"
            className="button-dialog"
            label={t("RequestNewButton")}
            size="small"
            primary={false}
            onClick={this.getNewBackupCodes}
          />
        </ModalDialog.Footer>
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
  getNewBackupCodes: PropTypes.func.isRequired,
  backupCodes: PropTypes.array.isRequired,
  backupCodesCount: PropTypes.number.isRequired,
  setBackupCodes: PropTypes.func.isRequired,
};

export default BackupCodesDialog;
