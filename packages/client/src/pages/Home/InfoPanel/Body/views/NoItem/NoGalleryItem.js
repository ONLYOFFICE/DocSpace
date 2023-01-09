import React from "react";
import styled from "styled-components";
import Text from "@docspace/components/text";

const StyledGalleryEmptyScreen = styled.div`
  .info-panel_gallery-empty-screen-img {
    display: block;
    margin: 0 auto;
    padding: 56px 0 48px 0;
  }
`;

const NoGalleryItem = ({ t }) => {
  return (
    <StyledGalleryEmptyScreen className="info-panel_gallery-empty-screen">
      <img
        className="info-panel_gallery-empty-screen-img"
        src="images/form-gallery-search.react.svg"
        alt="Empty Screen Gallery image"
      />
      <Text textAlign="center">
        {t("FormGallery:GalleryEmptyScreenDescription")}
      </Text>
    </StyledGalleryEmptyScreen>
  );
};
export default NoGalleryItem;
