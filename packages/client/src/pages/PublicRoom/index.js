import React, { useEffect } from "react";
import { observer, inject } from "mobx-react";
import { useLocation } from "react-router-dom";
import Section from "@docspace/common/components/Section";
import Loader from "@docspace/components/loader";
import { ValidationResult } from "../../helpers/constants";

import SectionHeaderContent from "./Header";
import SectionFilterContent from "./Filter";
import SectionBodyContent from "./Body";

import RoomPassword from "./sub-components/RoomPassword";

const PublicRoom = (props) => {
  const { isLoaded, isLoading, roomStatus, withPaging, validatePublicRoomKey } =
    props;

  const location = useLocation();
  const key = location.search.substring(5, location.search.length);

  useEffect(() => {
    validatePublicRoomKey(key);
  }, [validatePublicRoomKey]);

  const roomPage = () => (
    <Section
      withBodyScroll
      // withBodyAutoFocus={!isMobile}
      withPaging={withPaging}
    >
      <Section.SectionHeader>
        <SectionHeaderContent />
      </Section.SectionHeader>

      <Section.SectionFilter>
        <SectionFilterContent />
      </Section.SectionFilter>

      <Section.SectionBody>
        <SectionBodyContent />
      </Section.SectionBody>
    </Section>
  );

  const renderLoader = () => {
    return (
      <Section>
        <Section.SectionBody>
          <Loader className="pageLoader" type="rombs" size="40px" />
        </Section.SectionBody>
      </Section>
    );
  };

  const renderPage = () => {
    switch (roomStatus) {
      case ValidationResult.Ok:
        return roomPage();
      case ValidationResult.Invalid:
        return <></>;
      case ValidationResult.Expired:
        return <></>;
      case ValidationResult.Password:
        return <RoomPassword roomKey={key} />;

      default:
        return renderLoader();
    }
  };

  return isLoading ? renderLoader() : isLoaded ? roomPage() : renderPage();
};

export default inject(({ auth, publicRoomStore }) => {
  const { withPaging } = auth.settingsStore;
  const { validatePublicRoomKey, isLoaded, isLoading, roomStatus } =
    publicRoomStore;

  return {
    isLoaded,
    isLoading,
    roomStatus,

    withPaging,
    validatePublicRoomKey,
  };
})(observer(PublicRoom));
