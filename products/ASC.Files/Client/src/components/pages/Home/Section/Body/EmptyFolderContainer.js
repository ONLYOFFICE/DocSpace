import React from "react";
import styled from "styled-components";
import { EmptyScreenContainer } from "asc-web-components";

const EmptyFolderWrapper = styled.div`
  .empty-folder_container {
    .ec-image {
      padding-right: 16px;
      margin: 0 0 0 auto;
    }

    .empty-folder_link {
      margin-right: 8px;
    }

    .empty-folder_container-links {
      display: flex;
      margin: 12px 0;
    }

    .empty-folder_container_up-image {
      margin: 1px 8px 0 0;
      cursor: pointer;
    }

    .empty-folder_container_plus-image {
      margin: -8px 8px 0 0;
      line-height: unset;
    }

    .empty-folder_container-icon {
      height: 20px;
      width: 12px;
      margin-right: 4px;
    }
  }
`;

const EmptyFoldersContainer = props => {
  const imageAlt = "Empty folder image";
  const {
    imageSrc,
    headerText,
    subheadingText,
    descriptionText,
    buttons,
    widthProp
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
        widthProp={widthProp}
      />
    </EmptyFolderWrapper>
  );
};

export default EmptyFoldersContainer;
