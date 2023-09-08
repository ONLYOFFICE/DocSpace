import { useState, useEffect } from "react";
import { inject, observer } from "mobx-react";

import FileInput from "@docspace/components/file-input";

import FilesSelector from "../FilesSelector";
import { StyledBodyWrapper } from "./StyledComponents";

const FilesSelectorInput = (props) => {
  const {
    id,
    isThirdParty,
    isRoomsOnly,
    setNewPath,
    newPath,
    onSelectFolder: setSelectedFolder,
    onSelectFile: setSelectedFile,
    setBasePath,
    basePath,
    isDisabled,
    isError,
    toDefault,
    maxWidth,
    withoutInitPath,
    rootThirdPartyId,
    isErrorPath,

    filterParam,
    descriptionText,
  } = props;

  const isFilesSelection = !!filterParam;

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

  const onSelectFile = (fileInfo, folders) => {
    console.log("onSelectFile", fileInfo, folders);

    setSelectedFile && setSelectedFile(fileInfo);
    folders && setNewPath(folders, fileInfo?.title);
  };

  const filesSelectionProps = {
    onSelectFile: onSelectFile,
    filterParam: filterParam,
  };

  const foldersSelectionProps = {
    onSelectFolder: onSelectFolder,
    onSetBaseFolderPath: onSetBasePath,
  };

  return (
    <StyledBodyWrapper maxWidth={maxWidth}>
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
        descriptionText={descriptionText}
        filterParam={filterParam}
        rootThirdPartyId={rootThirdPartyId}
        isThirdParty={isThirdParty}
        isRoomsOnly={isRoomsOnly}
        id={id}
        onClose={onClose}
        isPanelVisible={isPanelVisible}
        {...(isFilesSelection ? filesSelectionProps : foldersSelectionProps)}
      />
    </StyledBodyWrapper>
  );
};

export default inject(({ filesSelectorInput }) => {
  const { basePath, newPath, setNewPath, setBasePath, toDefault, isErrorPath } =
    filesSelectorInput;

  return {
    isErrorPath,
    setBasePath,
    basePath,
    newPath,
    setNewPath,
    toDefault,
  };
})(observer(FilesSelectorInput));
