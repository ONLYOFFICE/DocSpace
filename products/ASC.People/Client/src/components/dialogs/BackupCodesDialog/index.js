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
    console.log("Render BackupCodesDialog");
    const { t, visible, onClose } = this.props;
    const count = 5; //TODO: get count from api
    const codes = ["qdf45g", "fg56dfg", "ugi8fm", "gfuti8f", "fkuidop"]; //TODO: get codes from api

    return (
      <ModalDialogContainer>
        <ModalDialog visible={visible} onClose={onClose}>
          <ModalDialog.Header>{t("BackupCodesTitle")}</ModalDialog.Header>
          <ModalDialog.Body>
            <div id="backup-codes-print-content">
              <Text className="text-dialog">{t("BackupCodesDescription")}</Text>
              <Text className="text-dialog">
                {t("BackupCodesSecondDescription")}
              </Text>

              <Trans
                t={t}
                i18nKey="CodesCounter"
                ns="BackupCodesDialog"
                count={count}
              >
                <Text className="text-dialog">
                  <strong>{{ count }} codes:</strong>
                </Text>
              </Trans>
              <Text className="text-dialog" isBold={true}>
                {codes.map((item) => {
                  return (
                    <strong key={item}>
                      {item} <br />
                    </strong>
                  );
                })}
              </Text>
            </div>
          </ModalDialog.Body>
          <ModalDialog.Footer>
            <Button
              key="PrintBtn"
              label={t("PrintButton")}
              size="medium"
              primary={true}
              onClick={this.printPage}
            />
            <Button
              key="RequestNewBtn"
              className="button-dialog"
              label={t("RequestNewButton")}
              size="medium"
              primary={false}
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
