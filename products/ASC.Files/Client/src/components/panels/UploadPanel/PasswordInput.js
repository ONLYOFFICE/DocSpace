import React, { useState } from "react";
import PasswordInput from "@appserver/components/password-input";
import Button from "@appserver/components/button";
import styled, { css } from "styled-components";
import { inject, observer } from "mobx-react";

const StyledBody = styled.div`
  display: flex;
  width: 468px;

  #conversion-button {
    margin-left: 8px;
  }
  #conversion-password {
    width: 382px;
  }
`;
const PasswordComponent = ({
  item,
  convertFile,
  removeFileFromList,
  onHideInput,
  uploadedFiles,
}) => {
  const [password, setPassword] = useState();

  const onClick = () => {
    console.log("on click", item);

    let index;

    uploadedFiles.reduce((acc, rec, id) => {
      if (rec.fileId === item.fileId) index = id;
    }, []);

    const newItem = {
      fileId: item.fileId,
      toFolderId: item.toFolderId,
      action: "convert",
      fileInfo: item.fileInfo,
      password: password,
      index,
    };

    onHideInput();
    removeFileFromList(item.fileId);
    convertFile(newItem);
  };

  const onChangePassword = (e) => {
    setPassword(e.target.value);
  };

  return (
    <StyledBody>
      <PasswordInput
        simpleView
        id="conversion-password"
        type="password"
        inputValue={password}
        onChange={onChangePassword}
      />
      <Button
        id="conversion-button"
        size="medium"
        scale
        primary
        label={"Button Text"}
        onClick={onClick}
      />
    </StyledBody>
  );
};

export default inject(({ uploadDataStore }) => {
  const {
    convertFile,
    removeFileFromList,
    files: uploadedFiles,
  } = uploadDataStore;

  return {
    uploadedFiles,
    removeFileFromList,
    convertFile,
  };
})(observer(PasswordComponent));
