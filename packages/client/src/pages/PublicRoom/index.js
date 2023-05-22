import React, { useEffect } from "react";
import { observer, inject } from "mobx-react";
import { useLocation } from "react-router-dom";
import Section from "@docspace/common/components/Section";
import Loader from "@docspace/components/loader";
import { ValidationResult } from "../../helpers/constants";

import SectionHeaderContent from "../Home/Section/Header";
import SectionFilterContent from "../Home/Section/Filter";
import SectionBodyContent from "../Home/Section/Body";
// import SectionHeaderContent from "./Header";
// import SectionFilterContent from "./Filter";
// import SectionBodyContent from "./Body";

import RoomPassword from "./sub-components/RoomPassword";
import RoomErrors from "./sub-components/RoomErrors";

import { RoomSharingDialog } from "../../components/dialogs";

const PublicRoom = (props) => {
  const {
    isLoaded,
    isLoading,
    roomStatus,
    roomId,
    withPaging,
    validatePublicRoomKey,
    fetchFiles,
    setRoomHref,
  } = props;

  const location = useLocation();
  const key = location.search.substring(5, location.search.length);

  useEffect(() => {
    validatePublicRoomKey(key);
  }, [validatePublicRoomKey]);

  useEffect(() => {
    isLoaded &&
      fetchFiles(roomId).then((res) => {
        setRoomHref(res?.links[0]?.href);
      });
  }, [fetchFiles, isLoaded]);

  const roomPage = () => (
    <>
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
      <RoomSharingDialog />
    </>
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
        return <RoomErrors isInvalid />;
      case ValidationResult.Expired:
        return <RoomErrors />;
      case ValidationResult.Password:
        return <RoomPassword roomKey={key} />;

      default:
        return renderLoader();
    }
  };

  return isLoading ? renderLoader() : isLoaded ? roomPage() : renderPage();
};

export default inject(({ auth, filesStore, publicRoomStore }) => {
  const { withPaging } = auth.settingsStore;
  const {
    validatePublicRoomKey,
    isLoaded,
    isLoading,
    roomStatus,
    roomId,
    setRoomHref,
  } = publicRoomStore;

  const { fetchFiles } = filesStore;

  return {
    roomId,
    isLoaded,
    isLoading,
    roomStatus,
    fetchFiles,
    setRoomHref,

    withPaging,
    validatePublicRoomKey,
  };
})(observer(PublicRoom));
