import React from "react";
import styled from "styled-components";
import { EmptyScreenContainer } from "asc-web-components";

const EmptyFolderWrapper = styled.div`
  .empty-folder_container {
    .empty-folder_link {
      margin-right: 7px;
    }

    .empty-folder_container-links {
      display: grid;
      margin: 8px 0;
      grid-template-columns: 12px 1fr;
      grid-column-gap: 8px;
    }

    .flex-wrapper_container {
      display: flex;
      flex-wrap: wrap;
    }

    .empty-folder_container_up-image {
      margin: 1px 8px 0 0;
      cursor: pointer;
    }

    .empty-folder_container_plus-image {
      display: flex;
      margin: -10px 8px 0 0;
      line-height: unset;
    }

    .empty-folder_container-icon {
      height: 20px;
      width: 12px;
      margin-right: 4px;
    }
  }
`;

const EmptyFoldersContainer = (props) => {
  const imageAlt = "Empty folder image";
  const {
    imageSrc,
    headerText,
    subheadingText,
    descriptionText,
    buttons,
  } = props;

  return (
    <EmptyFolderWrapper>
      <EmptyScreenContainer
        className="empty-folder_container"
        imageSrc={imageSrc}
        imageAlt={imageAlt}
        headerText={headerText}
        subheadingText={subheadingText}
        descriptionText={descriptionText}
        buttons={buttons}
      />
    </EmptyFolderWrapper>
  );
};

export default EmptyFoldersContainer;
