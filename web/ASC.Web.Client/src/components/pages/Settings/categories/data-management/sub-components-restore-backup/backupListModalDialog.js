import React from "react";
import PropTypes from "prop-types";
import ModalDialog from "@appserver/components/modal-dialog";
import Text from "@appserver/components/text";
import Button from "@appserver/components/button";
import FileListBody from "files/FileListBody";
import { getFiles } from "../../../../../../../../../packages/asc-web-common/api/files";
import { StyledBackupList } from "../styled-backup";
import { withTranslation } from "react-i18next";
import IconButton from "@appserver/components/icon-button";
import utils from "@appserver/components/utils";
import Aside from "@appserver/components/aside";
import Heading from "@appserver/components/heading";
import Backdrop from "@appserver/components/backdrop";
import Link from "@appserver/components/link";
import throttle from "lodash/throttle";
const { desktop } = utils.device;
const zIndex = 310;
class BackupListModalDialog extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      isLoading: false,
      filesList: [],
      hasNextPage: true,
      isNextPageLoading: false,
      displayType: this.getDisplayType(),
    };
    this.throttledResize = throttle(this.setDisplayType, 300);
  }
  componentDidMount() {
    const { isVisibleDialog } = this.props;

    if (isVisibleDialog) {
      window.addEventListener("resize", this.throttledResize);
    }
  }

  componentWillUnmount() {
    if (this.throttledResize) {
      this.throttledResize && this.throttledResize.cancel();
      window.removeEventListener("resize", this.throttledResize);
    }
  }
  getDisplayType = () => {
    const displayType =
      window.innerWidth < desktop.match(/\d+/)[0] ? "aside" : "modal";

    return displayType;
  };
  setDisplayType = () => {
    const displayType = this.getDisplayType();

    this.setState({ displayType: displayType });
  };
  loadNextPage = ({ startIndex = 0 }) => {
    //debugger;
    console.log(`loadNextPage(startIndex=${startIndex}")`);
    const { selectedFolder } = this.state;
    const pageCount = 30;
    console.log("selectedFolder", selectedFolder);
    this.setState({ isNextPageLoading: true }, () => {
      getFiles("12", pageCount, startIndex) //must be getbackuphistory method!
        .then((response) => {
          let newFilesList = startIndex
            ? this.state.filesList.concat(response.files)
            : response.files;
          console.log("newFilesList", newFilesList);

          this.setState({
            hasNextPage: newFilesList.length < response.total,
            isNextPageLoading: false,
            filesList: newFilesList,
          });
        })
        .catch((error) => console.log(error));
    });
  };
  render() {
    const { onModalClose, isVisibleDialog, t } = this.props;
    const {
      filesList,
      isNextPageLoading,
      hasNextPage,
      displayType,
    } = this.state;

    return displayType === "aside" ? (
      <>
        <Backdrop
          onClick={onModalClose}
          visible={isVisibleDialog}
          zIndex={zIndex}
          isAside={true}
        />
        <Aside visible={isVisibleDialog} zIndex={zIndex}>
          <StyledBackupList displayType={displayType}>
            <Heading size="medium" className="backup-list_aside-header_title">
              {t("BackupList")}
            </Heading>

            <div className="backup-list_aside-body_wrapper">
              <Text>{t("BackupListDeleteWarning")}</Text>
              <div className="backup-list_aside_body">
                <FileListBody
                  needRowSelection={false}
                  filesList={filesList}
                  hasNextPage={hasNextPage}
                  isNextPageLoading={isNextPageLoading}
                  loadNextPage={this.loadNextPage}
                  displayType={displayType}
                >
                  <div className="backup-list_options">
                    <Link className="backup-list_restore-link">
                      {t("RestoreBackup")}
                    </Link>

                    <IconButton
                      className="backup-list_trash-icon"
                      size={16}
                      color="#657077"
                      iconName="/static/images/button.trash.react.svg"
                      onClick={undefined}
                    />
                  </div>
                </FileListBody>
              </div>
            </div>
          </StyledBackupList>
        </Aside>
      </>
    ) : (
      <ModalDialog visible={isVisibleDialog} onClose={onModalClose}>
        <ModalDialog.Header>{t("BackupList")}</ModalDialog.Header>
        <ModalDialog.Body>
          <StyledBackupList>
            <div className="backup-list_modal-dialog_body">
              <Text>{t("BackupListDeleteWarning")}</Text>
              <FileListBody
                filesList={filesList}
                hasNextPage={hasNextPage}
                isNextPageLoading={isNextPageLoading}
                loadNextPage={this.loadNextPage}
                listHeight={250}
                needRowSelection={false}
                displayType={displayType}
              >
                <div className="backup-list_options">
                  <Text className="backup-list_restore-link">
                    {t("RestoreBackup")}
                  </Text>
                  <IconButton
                    className="backup-list_trash-icon"
                    size={16}
                    color="#657077"
                    iconName="/static/images/button.trash.react.svg"
                    onClick={undefined}
                  />
                </div>
              </FileListBody>
            </div>
          </StyledBackupList>
        </ModalDialog.Body>
        <ModalDialog.Footer>
          <Button
            className="modal-dialog-button"
            primary
            size="big"
            label={t("Common:CloseButton")}
            tabIndex={1}
            onClick={onModalClose}
          />
        </ModalDialog.Footer>
      </ModalDialog>
    );
  }
}

BackupListModalDialog.propTypes = {
  t: PropTypes.func.isRequired,
  onModalClose: PropTypes.func.isRequired,
  isVisibleDialog: PropTypes.bool.isRequired,
};

export default withTranslation(["Settings", "Common"])(BackupListModalDialog);
