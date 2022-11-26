import React, { useState, useEffect } from "react";
import { inject } from "mobx-react";
import { ReactSVG } from "react-svg";

import toastr from "@docspace/components/toast/toastr";
import { Button, Text, Textarea } from "@docspace/components";
import infoPanel from "@docspace/common/components/Section/sub-components/info-panel";

const CommentEditor = ({
  t,
  item,
  editing,
  setSelection,
  fetchFileVersions,
  updateCommentVersion,
  canChangeVersionFileHistory,
  setVerHistoryFileId,
  setVerHistoryFileAccess,
}) => {
  const { id, comment, version, access, folderType } = item;

  const changeVersionHistoryAbility = canChangeVersionFileHistory({
    access,
    folderType,
    editing,
  });

  useEffect(() => {
    setVerHistoryFileId(id);
    setVerHistoryFileAccess(access);
  }, []);

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

    await fetchFileVersions(id, access).catch((err) => {
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
          {changeVersionHistoryAbility && (
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

export default inject(({ auth, versionHistoryStore, accessRightsStore }) => {
  const { setSelection } = auth.infoPanelStore;

  const {
    fetchFileVersions,
    updateCommentVersion,
    isEditingVersion,
    isEditing,
    fileId,
    setVerHistoryFileId,
    setVerHistoryFileAccess,
  } = versionHistoryStore;

  const { canChangeVersionFileHistory } = accessRightsStore;
  const editing = isEditingVersion || isEditing;

  return {
    setSelection,
    fetchFileVersions,
    updateCommentVersion,
    canChangeVersionFileHistory,
    editing,
    setVerHistoryFileId,
    setVerHistoryFileAccess,
  };
})(CommentEditor);
