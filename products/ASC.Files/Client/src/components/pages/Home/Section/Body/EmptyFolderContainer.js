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
      margin-right: 8px;
      cursor: pointer;
    }

    .empty-folder_container_plus-image {
      margin: -8px 8px 0 0;
    }

    .empty-folder_container-icon {
      margin-right: 4px;
    }
  }
`;

const EmptyFoldersContainer = props => {
  const imageAlt = "Empty folder image";

  return (
    <EmptyFolderWrapper>
      <EmptyScreenContainer
        className="empty-folder_container"
        imageSrc={props.imageSrc}
        imageAlt={imageAlt}
        headerText={props.headerText}
        subheadingText={props.subheadingText}
        descriptionText={props.descriptionText}
        buttons={props.buttons}
      />
    </EmptyFolderWrapper>
  );
};

export default EmptyFoldersContainer;
