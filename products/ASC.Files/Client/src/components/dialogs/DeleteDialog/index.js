import React, { memo } from "react";
import ModalDialogContainer from "../ModalDialogContainer";
import {
  toastr,
  ModalDialog,
  Button,
  Text,
  Checkbox,
  CustomScrollbarsVirtualList
} from "asc-web-components";
import { withTranslation } from "react-i18next";
import { FixedSizeList as List, areEqual } from "react-window";
import AutoSizer from "react-virtualized-auto-sizer";
import i18n from "./i18n";
import { api, utils } from "asc-web-common";

const { files } = api;
const { changeLanguage } = utils;

class DeleteDialogComponent extends React.Component {
  constructor(props) {
    super(props);

    const foldersList = [];
    const filesList = [];
    const selection = [];

    let i = 0;
    while (props.selection.length !== i) {
      selection.push({ ...props.selection[i], checked: true });
      if (selection[i].fileExst) {
        filesList.push(selection[i]);
      } else {
        foldersList.push(selection[i]);
      }
      i++;
    }
    changeLanguage(i18n);

    this.state = { isLoading: false, foldersList, filesList, selection };
  }

  onDelete = () => {
    const { isRecycleBinFolder, onClose } = this.props;
    const { selection } = this.state;

    const deleteAfter = true; //Delete after finished
    const immediately = isRecycleBinFolder ? true : false; //Don't move to the Recycle Bin
    const successMessage = "Files and folders was deleted";

    const folderIds = [];
    const fileIds = [];

    
    let i = 0;
    while (selection.length !== i) {
      if (selection[i].fileExst && selection[i].checked) {
        fileIds.push(selection[i].id.toString());
      } else if(selection[i].checked) {
        folderIds.push(selection[i].id.toString());
      }
      i++;
    }

    console.log("fileIds", fileIds);
    console.log("folderIds", folderIds);

    //this.setState({ isLoading: true }, () => {
    //  files
    //    .removeFiles(folderIds, fileIds, deleteAfter, immediately)
    //    .then(() => {
    //      toastr.success(successMessage);
    //    })
    //    .catch(err => {
    //      toastr.error(err);
    //    })
    //    .finally(() => {
    //      this.setState({ isLoading: false }, () => onClose());
    //    });
    //});
  };

  onChange = event => {
    const value = event.target.value.split("/");
    const fileType = value[0];
    const id = Number(value[1]);

    const newSelection = this.state.selection;

    if (fileType !== "undefined") {
      const a = newSelection.find(x => x.id === id && x.fileExst);
      a.checked = !a.checked;
      this.setState({ selection: newSelection });
    } else {

      const a = newSelection.find(x => x.id === id && !x.fileExst);
      a.checked = !a.checked;
      this.setState({ selection: newSelection });
    }
  };

  render() {
    const { onClose, visible, t, selection } = this.props;
    const { isLoading, filesList, foldersList } = this.state;

    const renderItems = memo(({ data, index, style }) => {
      return (
        <Checkbox
          truncate
          style={style}
          className="modal-dialog-checkbox"
          //value={data[index].id}
          value={`${data[index].fileExst}/${data[index].id}`}
          onChange={this.onChange}
          key={`checkbox_${index}`}
          isChecked={data[index].checked}
          label={data[index].title}
        />
      );
    }, areEqual);

    const renderList = ({ height, width }) => {
      const filesHeight = filesList.length * 25;
      const foldersHeight = foldersList.length * 25;

      return (
        <>
          <Text isBold>Folders:</Text>
          <List
            className="List"
            height={foldersHeight}
            width={width}
            itemSize={25}
            itemCount={foldersList.length}
            itemData={foldersList}
            outerElementType={CustomScrollbarsVirtualList}
          >
            {renderItems}
          </List>

          <Text isBold className="delete_dialog-header-text">
            Files:
          </Text>
          <List
            className="List"
            height={filesHeight}
            width={width}
            itemSize={25}
            itemCount={filesList.length}
            itemData={filesList}
            outerElementType={CustomScrollbarsVirtualList}
          >
            {renderItems}
          </List>
        </>
      );
    };

    const containerStyles = {
      height: selection.length * 25 + 50,
      maxHeight: 480
    };
    return (
      <ModalDialogContainer>
        <ModalDialog
          visible={visible}
          onClose={onClose}
          headerContent={t("ConfirmationTitle")}
          bodyContent={
            <>
              <Text>{t("DeleteDialogQuestion")}</Text>
              <div style={containerStyles} className="modal-dialog-content">
                <AutoSizer>{renderList}</AutoSizer>
              </div>
            </>
          }
          footerContent={
            <>
              <Button
                key="OkButton"
                label={t("OKButton")}
                size="medium"
                primary
                onClick={this.onDelete}
                isLoading={isLoading}
              />
              <Button
                className="button-dialog"
                key="CancelButton"
                label={t("CancelButton")}
                size="medium"
                onClick={onClose}
                isLoading={isLoading}
              />
            </>
          }
        />
      </ModalDialogContainer>
    );
  }
}

const ModalDialogContainerTranslated = withTranslation()(DeleteDialogComponent);

const DeleteDialog = props => (
  <ModalDialogContainerTranslated i18n={i18n} {...props} />
);

export default DeleteDialog;
