import React from "react";
import Backdrop from "@docspace/components/backdrop";
import Heading from "@docspace/components/heading";
import Aside from "@docspace/components/aside";
import Button from "@docspace/components/button";
import Text from "@docspace/components/text";
import Link from "@docspace/components/link";
import { withTranslation } from "react-i18next";
import toastr from "@docspace/components/toast/toastr";
import {
  StyledAsidePanel,
  StyledContent,
  StyledFooter,
  StyledHeaderContent,
  StyledBody,
} from "../StyledPanels";
import { inject, observer } from "mobx-react";
import Loaders from "@docspace/common/components/Loaders";
import withLoader from "../../../HOCs/withLoader";

class ChangeOwnerComponent extends React.Component {
  constructor(props) {
    super(props);

    const owner = props.selection[0].createdBy;
    this.state = { showPeopleSelector: false, owner };
  }

  onOwnerChange = () => {
    const { owner } = this.state;
    const { selection, setFolder, setFile, setIsLoading, setFilesOwner } =
      this.props;
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
          setFolder(res[0]);
        } else {
          setFile(res[0]);
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
    this.props.setBufferSelection(null);
    this.props.setChangeOwnerPanelVisible(false);
  };

  render() {
    const { visible, t, selection, isLoading } = this.props;
    const { owner } = this.state;

    const ownerName = owner.displayName ? owner.displayName : owner.label;
    const fileName = selection[0]?.title;
    const id = owner.id ? owner.id : owner.key;
    const disableSaveButton = owner && selection[0]?.createdBy.id === id;
    const zIndex = 310;

    return (
      <StyledAsidePanel visible={visible}>
        <Backdrop
          onClick={this.onClose}
          visible={visible}
          zIndex={zIndex}
          isAside
        />
        <Aside
          className="header_aside-panel"
          visible={visible}
          onClose={this.onClose}
        >
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
            <StyledFooter>
              <Button
                label={t("Common:SaveButton")}
                size="small"
                scale
                primary
                onClick={this.onOwnerChange}
                isDisabled={disableSaveButton || isLoading}
              />
            </StyledFooter>
          </StyledContent>
        </Aside>
      </StyledAsidePanel>
    );
  }
}

const ChangeOwnerPanel = withTranslation(["ChangeOwnerPanel", "Common"])(
  withLoader(ChangeOwnerComponent)(<Loaders.DialogAsideLoader isPanel />)
);

export default inject(
  ({ auth, filesStore, dialogsStore, clientLoadingStore }) => {
    const {
      selection,
      bufferSelection,
      setFile,
      setFolder,
      setFilesOwner,

      setBufferSelection,
    } = filesStore;
    const { ownerPanelVisible, setChangeOwnerPanelVisible } = dialogsStore;

    return {
      theme: auth.settingsStore.theme,
      selection: selection.length ? selection : [bufferSelection],
      isLoading: clientLoadingStore.isLoading,
      visible: ownerPanelVisible,

      setFile,
      setFolder,
      setIsLoading: clientLoadingStore.setIsSectionBodyLoading,
      setChangeOwnerPanelVisible,
      setFilesOwner,
      setBufferSelection,
    };
  }
)(observer(ChangeOwnerPanel));
