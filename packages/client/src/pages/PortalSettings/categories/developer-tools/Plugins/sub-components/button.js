import React from "react";
import styled from "styled-components";

import Button from "@docspace/components/button";

const StyledUploadButton = styled.div`
  width: auto;
`;

const UploadButton = ({ t, addPlugin }) => {
  const pluginInputRef = React.useRef(null);

  const onAddAction = () => {
    pluginInputRef.current.click();
  };

  const onInputClick = (e) => {
    e.target.value = null;
  };

  const onFileChange = (e) => {
    let formData = new FormData();

    formData.append("file", e.target.files[0]);

    addPlugin(formData);
  };

  return (
    <StyledUploadButton>
      <Button
        className={"add-button"}
        label={t("UploadPlugin")}
        primary
        size={"small"}
        scale={false}
        onClick={onAddAction}
      />
      <input
        id="customPluginInput"
        className="custom-file-input"
        type="file"
        accept=".zip"
        onChange={onFileChange}
        onClick={onInputClick}
        ref={pluginInputRef}
        style={{ display: "none" }}
      />
    </StyledUploadButton>
  );
};

export default UploadButton;
