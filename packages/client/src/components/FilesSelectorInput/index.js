import { useState, useEffect } from "react";
import FileInput from "@docspace/components/file-input";
import FilesSelector from "../FilesSelector";
import styled from "styled-components";
import { inject, observer } from "mobx-react";

const StyledComponent = styled.div`
  max-width: ${(props) => (props.maxWidth ? props.maxWidth : "350px")};
`;

const FilesSelectorInput = (props) => {
  const {
    id,
    isThirdParty,
    isRoomsOnly,
    setNewPath,
    newPath,
    onSelectFolder: setSelectedFolder,
    setBasePath,
    basePath,
    isDisabled,
    isError,
    toDefault,
    maxWidth,
    withoutInitPath,
    rootThirdPartyId,
    isErrorPath,
  } = props;

  const [isPanelVisible, setIsPanelVisible] = useState(false);
  const [isLoading, setIsLoading] = useState(!!id && !withoutInitPath);

  useEffect(() => {
    return () => toDefault();
  }, []);

  const onClick = () => {
    setIsPanelVisible(true);
  };

  const onClose = () => {
    setIsPanelVisible(false);
  };

  const onSetBasePath = (folders) => {
    console.log("onSetBasePath", withoutInitPath);
    !withoutInitPath && setBasePath(folders);
    isLoading && setIsLoading(false);
  };

  const onSelectFolder = (folderId, folders) => {
    console.log("onSelectFolder", folderId, folders);

    setSelectedFolder && setSelectedFolder(folderId);

    folders && setNewPath(folders);
  };

  return (
    <StyledComponent maxWidth={maxWidth}>
      <FileInput
        onClick={onClick}
        fromStorage
        path={newPath || basePath}
        isLoading={isLoading}
        isDisabled={isDisabled || isLoading}
        hasError={isError || isErrorPath}
        scale
      />

      <FilesSelector
        rootThirdPartyId={rootThirdPartyId}
        isThirdParty={isThirdParty}
        isRoomsOnly={isRoomsOnly}
        id={id}
        onClose={onClose}
        isPanelVisible={isPanelVisible}
        onSetBaseFolderPath={onSetBasePath}
        onSelectFolder={onSelectFolder}
      />
    </StyledComponent>
  );
};

export default inject(({ selectFolderDialogStore }) => {
  const { basePath, newPath, setNewPath, setBasePath, toDefault, isErrorPath } =
    selectFolderDialogStore;

  return {
    isErrorPath,
    setBasePath,
    basePath,
    newPath,
    setNewPath,
    toDefault,
  };
})(observer(FilesSelectorInput));
