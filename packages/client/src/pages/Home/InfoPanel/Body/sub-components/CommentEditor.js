import PencilReactSvgUrl from "PUBLIC_DIR/images/pencil.react.svg?url";
import React, { useState, useEffect } from "react";
import { inject } from "mobx-react";
import { ReactSVG } from "react-svg";

import toastr from "@docspace/components/toast/toastr";
import { Button, Text, Textarea } from "@docspace/components";
import { MAX_FILE_COMMENT_LENGTH } from "@docspace/common/constants";
// import infoPanel from "@docspace/common/components/Section/sub-components/info-panel";

const CommentEditor = ({
  t,
  item,
  editing,
  setSelection,
  fetchFileVersions,
  updateCommentVersion,

  setVerHistoryFileId,
  setVerHistoryFileSecurity,
}) => {
  const { id, comment, version, security } = item;

  const changeVersionHistoryAbility = !editing && security?.EditHistory;

  useEffect(() => {
    setVerHistoryFileId(id);
    setVerHistoryFileSecurity(security);
  }, []);

  const [isEdit, setIsEdit] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const [inputValue, setInputValue] = useState(comment || "");

  const onChangeInputValue = (e) => {
    const value = e.target.value;
    if (value.length > MAX_FILE_COMMENT_LENGTH) return;

    setInputValue(value);
  };

  const onOpenEditor = async () => {
    setInputValue(comment);
    setIsEdit(true);
  };

  const onSave = async () => {
    setIsLoading(true);

    await fetchFileVersions(id, security).catch((err) => {
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
              <ReactSVG className="edit_toggle-icon" src={PencilReactSvgUrl} />
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

export default inject(({ auth, versionHistoryStore }) => {
  const { setSelection } = auth.infoPanelStore;

  const {
    fetchFileVersions,
    updateCommentVersion,
    isEditingVersion,
    isEditing,
    fileId,
    setVerHistoryFileId,
    setVerHistoryFileSecurity,
  } = versionHistoryStore;

  const editing = isEditingVersion || isEditing;

  return {
    setSelection,
    fetchFileVersions,
    updateCommentVersion,

    editing,
    setVerHistoryFileId,
    setVerHistoryFileSecurity,
  };
})(CommentEditor);
