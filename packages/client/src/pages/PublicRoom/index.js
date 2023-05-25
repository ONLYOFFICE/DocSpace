import React, { useEffect } from "react";
import { observer, inject } from "mobx-react";
import { useLocation } from "react-router-dom";
import Section from "@docspace/common/components/Section";
import Loader from "@docspace/components/loader";
import { ValidationResult } from "../../helpers/constants";

import SectionHeaderContent from "../Home/Section/Header";
import SectionFilterContent from "../Home/Section/Filter";
import SectionBodyContent from "../Home/Section/Body";

import RoomPassword from "./sub-components/RoomPassword";
import RoomErrors from "./sub-components/RoomErrors";

import { RoomSharingDialog } from "../../components/dialogs";
import SelectionArea from "../Home/SelectionArea";

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
    getFilesSettings,

    showSecondaryProgressBar,
    secondaryProgressBarValue,
    secondaryProgressBarIcon,
    showSecondaryButtonAlert,
  } = props;

  const location = useLocation();
  const key = location.search.substring(5, location.search.length);

  useEffect(() => {
    validatePublicRoomKey(key);
  }, [validatePublicRoomKey]);

  const fetchRoomFiles = async () => {
    await getFilesSettings(key);

    fetchFiles(roomId).then((res) => {
      setRoomHref(res?.links[0]?.href);
    });
  };

  useEffect(() => {
    if (isLoaded) fetchRoomFiles();
  }, [fetchFiles, isLoaded]);

  const sectionProps = {
    showSecondaryProgressBar,
    secondaryProgressBarValue,
    secondaryProgressBarIcon,
    showSecondaryButtonAlert,
  };

  const roomPage = () => (
    <>
      <Section
        withBodyScroll
        // withBodyAutoFocus={!isMobile}
        withPaging={withPaging}
        {...sectionProps}
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
      <SelectionArea />
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

export default inject(
  ({ auth, filesStore, publicRoomStore, uploadDataStore, settingsStore }) => {
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
    const { getFilesSettings } = settingsStore;

    const {
      visible: showSecondaryProgressBar,
      percent: secondaryProgressBarValue,
      icon: secondaryProgressBarIcon,
      alert: showSecondaryButtonAlert,
    } = uploadDataStore.secondaryProgressDataStore;

    return {
      roomId,
      isLoaded,
      isLoading,
      roomStatus,
      fetchFiles,
      setRoomHref,
      getFilesSettings,

      withPaging,
      validatePublicRoomKey,

      showSecondaryProgressBar,
      secondaryProgressBarValue,
      secondaryProgressBarIcon,
      showSecondaryButtonAlert,
    };
  }
)(observer(PublicRoom));
