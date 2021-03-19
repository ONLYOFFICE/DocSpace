import React from "react";
import { withRouter } from "react-router";
import {
  Button,
  Heading,
  Link,
  Text,
  Backdrop,
  Aside,
} from "asc-web-components";
import { withTranslation } from "react-i18next";
import { toastr } from "asc-web-common";
import OwnerSelector from "./OwnerSelector";
import {
  StyledAsidePanel,
  StyledContent,
  StyledFooter,
  StyledHeaderContent,
  StyledBody,
} from "../StyledPanels";
import { inject, observer } from "mobx-react";

class ChangeOwnerComponent extends React.Component {
  constructor(props) {
    super(props);

    const owner = props.selection[0].createdBy;
    this.state = { showPeopleSelector: false, owner };
  }

  updateRowData = (newRowData) => {
    const { files, folders, setFiles, setFolders } = this.props;

    for (let item of newRowData) {
      if (!item.fileExst && item.foldersCount) {
        let folderIndex = folders.findIndex((x) => x.id === item.id);
        if (folderIndex !== -1) {
          folders[folderIndex] = item;
        }
      } else {
        let fileIndex = files.findIndex((x) => x.id === item.id);
        if (fileIndex !== -1) {
          files[fileIndex] = item;
        }
      }
    }

    setFiles(files);
    setFolders(folders);
  };

  onOwnerChange = () => {
    const { owner } = this.state;
    const {
      files,
      folders,
      selection,
      setFolders,
      setFiles,
      setIsLoading,
      setFilesOwner,
    } = this.props;
    const folderIds = [];
    const fileIds = [];
    const selectedItem = selection[0];
    const ownerId = owner.id ? owner.id : owner.key;
    const isFolder = selectedItem.isFolder;

    isFolder ? folderIds.push(selectedItem.id) : fileIds.push(selectedItem.id);

    setIsLoading(true);
    setFilesOwner(folderIds, fileIds, ownerId)
      .then((res) => {
        if (isFolder) {
          let folderIndex = folders.findIndex((x) => x.id === selectedItem.id);
          if (folderIndex !== -1) {
            folders[folderIndex] = res[0];
          }
          setFolders(folders);
        } else {
          let fileIndex = files.findIndex((x) => x.id === selectedItem.id);
          if (fileIndex !== -1) {
            files[fileIndex] = res[0];
          }
          setFiles(files);
        }
      })
      .catch((err) => toastr.error(err))
      .finally(() => {
        this.onClose();
        setIsLoading(false);
      });
  };

  onOwnerSelect = (options) => {
    this.setState({ owner: options[0], showPeopleSelector: false });
  };

  onShowPeopleSelector = () => {
    this.setState({ showPeopleSelector: !this.state.showPeopleSelector });
  };

  onClose = () => {
    this.props.setChangeOwnerPanelVisible(false);
  };

  render() {
    const { visible, t, selection, groupsCaption, isLoading } = this.props;
    const { showPeopleSelector, owner } = this.state;

    const ownerName = owner.displayName ? owner.displayName : owner.label;
    const fileName = selection[0].title;
    const id = owner.id ? owner.id : owner.key;
    const disableSaveButton = owner && selection[0].createdBy.id === id;
    const zIndex = 310;

    return (
      <StyledAsidePanel visible={visible}>
        <Backdrop
          onClick={this.onClose}
          visible={visible}
          zIndex={zIndex}
          isAside
        />
        <Aside className="header_aside-panel">
          <StyledContent>
            <StyledHeaderContent>
              <Heading className="sharing_panel-header" size="medium" truncate>
                {t("ChangeOwner", { fileName })}
              </Heading>
            </StyledHeaderContent>
            <StyledBody>
              <div className="change-owner_body">
                <Link
                  className="change-owner_owner-label"
                  isHovered
                  type="action"
                  onClick={this.onShowPeopleSelector}
                >
                  {ownerName}
                </Link>
                <Text>{t("ChangeOwnerDescription")}</Text>
              </div>
            </StyledBody>
          </StyledContent>
          <StyledFooter>
            <Button
              label={t("AddButton")}
              size="medium"
              scale
              primary
              onClick={this.onOwnerChange}
              isDisabled={disableSaveButton || isLoading}
            />
          </StyledFooter>
        </Aside>
        {showPeopleSelector && (
          <OwnerSelector
            ownerLabel={ownerName}
            isOpen={showPeopleSelector}
            groupsCaption={groupsCaption}
            onOwnerSelect={this.onOwnerSelect}
            onClose={this.onClose}
            onClosePanels={this.onClosePanels}
          />
        )}
      </StyledAsidePanel>
    );
  }
}

const ChangeOwnerPanel = withTranslation("ChangeOwnerPanel")(
  ChangeOwnerComponent
);

export default inject(({ auth, initFilesStore, filesStore, dialogsStore }) => {
  const { setIsLoading, isLoading } = initFilesStore;
  const {
    files,
    folders,
    selection,
    setFiles,
    setFolders,
    setFilesOwner,
  } = filesStore;
  const { ownerPanelVisible, setChangeOwnerPanelVisible } = dialogsStore;

  return {
    groupsCaption: auth.settingsStore.customNames.groupsCaption,
    files,
    folders,
    selection,
    isLoading,
    visible: ownerPanelVisible,

    setFiles,
    setFolders,
    setIsLoading,
    setChangeOwnerPanelVisible,
    setFilesOwner,
  };
})(withRouter(observer(ChangeOwnerPanel)));
