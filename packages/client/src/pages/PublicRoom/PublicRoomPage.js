import React from "react";
import { inject, observer } from "mobx-react";
import { useNavigate, useLocation } from "react-router-dom";
import Section from "@docspace/common/components/Section";
import SectionHeaderContent from "../Home/Section/Header";
import SectionFilterContent from "../Home/Section/Filter";
import SectionBodyContent from "../Home/Section/Body";
import FilesPanels from "../../components/FilesPanels";

import { RoomSharingDialog } from "../../components/dialogs";
import SelectionArea from "../Home/SelectionArea/FilesSelectionArea";
import MediaViewer from "../Home/MediaViewer";

import { usePublic } from "../Home/Hooks";

const PublicRoomPage = (props) => {
  const {
    roomId,
    withPaging,
    fetchFiles,
    isEmptyPage,
    setIsLoading,

    showSecondaryProgressBar,
    secondaryProgressBarValue,
    secondaryProgressBarIcon,
    showSecondaryButtonAlert,
  } = props;

  const location = useLocation();

  usePublic({
    roomId,
    location,
    fetchFiles,
    setIsLoading,
  });

  const sectionProps = {
    showSecondaryProgressBar,
    secondaryProgressBarValue,
    secondaryProgressBarIcon,
    showSecondaryButtonAlert,
  };

  return (
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

        {!isEmptyPage && (
          <Section.SectionFilter>
            <SectionFilterContent />
          </Section.SectionFilter>
        )}

        <Section.SectionBody>
          <SectionBodyContent />
        </Section.SectionBody>
      </Section>

      <FilesPanels />
      <SelectionArea />
      <MediaViewer />
    </>
  );
};

export default inject(
  ({
    auth,
    filesStore,
    publicRoomStore,
    uploadDataStore,
    settingsStore,
    clientLoadingStore,
  }) => {
    const { withPaging } = auth.settingsStore;
    const { validatePublicRoomKey, isLoaded, isLoading, roomStatus, roomId } =
      publicRoomStore;

    const { fetchFiles, isEmptyPage } = filesStore;
    const { getFilesSettings } = settingsStore;

    const {
      visible: showSecondaryProgressBar,
      percent: secondaryProgressBarValue,
      icon: secondaryProgressBarIcon,
      alert: showSecondaryButtonAlert,
    } = uploadDataStore.secondaryProgressDataStore;

    const { setIsSectionFilterLoading, setIsSectionBodyLoading } =
      clientLoadingStore;

    const setIsLoading = (param) => {
      setIsSectionFilterLoading(param);
      setIsSectionBodyLoading(param);
    };

    return {
      roomId,
      isLoaded,
      isLoading,
      roomStatus,
      fetchFiles,
      getFilesSettings,

      withPaging,
      validatePublicRoomKey,

      showSecondaryProgressBar,
      secondaryProgressBarValue,
      secondaryProgressBarIcon,
      showSecondaryButtonAlert,

      isAuthenticated: auth.isAuthenticated,
      isEmptyPage,
      setIsLoading,
    };
  }
)(observer(PublicRoomPage));
