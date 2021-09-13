import React from "react";
import styled from "styled-components";
import EmptyScreenContainer from "@appserver/components/empty-screen-container";
import NoUserSelect from "@appserver/components/utils/commonStyles";

const EmptyFolderWrapper = styled.div`
  .empty-folder_container {
    .empty-folder_link {
      margin-right: 7px;
    }

    .empty-folder_container-links {
      display: grid;
      margin: 13px 0;
      grid-template-columns: 12px 1fr;
      grid-column-gap: 8px;
    }

    .flex-wrapper_container {
      display: flex;
      flex-wrap: wrap;
    }

    .empty-folder_container_up-image,
    .empty-folder_container_plus-image {
      margin: 4px 8px 0 0;
      cursor: pointer;
    }

    .empty-folder_container_plus-image {
      display: flex;
      line-height: unset;
      ${NoUserSelect}
    }

    .empty-folder_container-icon {
      height: 20px;
      width: 12px;
      margin: 4px 4px 0 0;
      cursor: pointer;
    }

    .empty-connect_container-links {
      position: relative;
      bottom: 16px;
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
