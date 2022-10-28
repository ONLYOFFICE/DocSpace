import React, { useState } from "react";
import { inject } from "mobx-react";
import { ReactSVG } from "react-svg";

import toastr from "@docspace/components/toast/toastr";
import { Button, Text, Textarea } from "@docspace/components";
import infoPanel from "@docspace/common/components/Section/sub-components/info-panel";

const CommentEditor = ({
  t,
  item,

  setSelection,
  isRecycleBinFolder,
  fetchFileVersions,
  updateCommentVersion,
}) => {
  const { id, comment, version } = item;

  const [isEdit, setIsEdit] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const [inputValue, setInputValue] = useState(comment || "");
  const onChangeInputValue = (e) => setInputValue(e.target.value);

  const onOpenEditor = async () => {
    setInputValue(comment);
    setIsEdit(true);
  };

  const onSave = async () => {
    setIsLoading(true);

    await fetchFileVersions(id).catch((err) => {
      toastr.error(err);
      setIsLoading(false);
    });

    updateCommentVersion(id, inputValue, version).catch((err) => {
      toastr.error(err);
      setIsLoading(false);
    });

    setSelection({ ...item, comment: inputValue });
    setIsEdit(false);
    setIsLoading(false);
  };

  const onCancel = () => {
    setIsEdit(false);
    setInputValue(comment);
  };

  return (
    <div className="property-comment_editor property-content">
      {!isEdit ? (
        <div className="property-comment_editor-display">
          {comment && (
            <Text truncate className="property-content">
              {comment}
            </Text>
          )}
          {!isRecycleBinFolder && (
            <div className="edit_toggle" onClick={onOpenEditor}>
              <ReactSVG
                className="edit_toggle-icon"
                src="images/pencil.react.svg"
              />
              <div className="property-content edit_toggle-text">
                {comment ? t("Common:EditButton") : t("Common:AddButton")}
              </div>
            </div>
          )}
        </div>
      ) : (
        <div className="property-comment_editor-editor">
          <Textarea
            isDisabled={isLoading}
            value={inputValue}
            onChange={onChangeInputValue}
            autoFocus
            areaSelect
            heightTextArea={54}
            fontSize={13}
          />
          <div className="property-comment_editor-editor-buttons">
            <Button
              isLoading={isLoading}
              label={t("Common:SaveButton")}
              onClick={onSave}
              size="extraSmall"
              primary
            />
            <Button
              label={t("Common:CancelButton")}
              onClick={onCancel}
              size="extraSmall"
            />
          </div>
        </div>
      )}
    </div>
  );
};

export default inject(({ auth, versionHistoryStore, treeFoldersStore }) => {
  const { setSelection } = auth.infoPanelStore;

  const { fetchFileVersions, updateCommentVersion } = versionHistoryStore;
  const { isRecycleBinFolder } = treeFoldersStore;
  return {
    setSelection,
    isRecycleBinFolder,
    fetchFileVersions,
    updateCommentVersion,
  };
})(CommentEditor);
