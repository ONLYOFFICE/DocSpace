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
import MediaViewer from "../Home/MediaViewer";

import FilesFilter from "@docspace/common/api/files/filter";

const PublicRoom = (props) => {
  const {
    isLoaded,
    isLoading,
    roomStatus,
    roomId,
    withPaging,
    validatePublicRoomKey,
    fetchFiles,
    getFilesSettings,

    showSecondaryProgressBar,
    secondaryProgressBarValue,
    secondaryProgressBarIcon,
    showSecondaryButtonAlert,
  } = props;

  const location = useLocation();
  const lastKeySymbol = location.search.indexOf("&");

  const lastIndex =
    lastKeySymbol === -1 ? location.search.length : lastKeySymbol;
  const key = location.search.substring(5, lastIndex);

  useEffect(() => {
    validatePublicRoomKey(key);
  }, [validatePublicRoomKey]);

  const setPublicRoomFilter = (filterObj) => {
    const newFilter = new FilesFilter();

    newFilter.filterType = filterObj.filterType;
    newFilter.page = filterObj.page;
    newFilter.pageCount = filterObj.pageCount;
    newFilter.search = filterObj.search;
    newFilter.sortBy = filterObj.sortBy;
    newFilter.sortOrder = filterObj.sortOrder;
    newFilter.total = filterObj.total;
    newFilter.viewAs = filterObj.viewAs;
    newFilter.withSubfolders = filterObj.withSubfolders;
    newFilter.folder = filterObj.folder;

    return newFilter;
  };

  const fetchRoomFiles = async () => {
    await getFilesSettings(key);

    const filterObj = FilesFilter.getFilter(window.location);

    if (filterObj?.folder && filterObj?.folder !== "@my") {
      const filter = setPublicRoomFilter(filterObj);

      fetchFiles(filter.folder, filter);
    } else {
      fetchFiles(roomId);
    }
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
      <MediaViewer />
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
    const { validatePublicRoomKey, isLoaded, isLoading, roomStatus, roomId } =
      publicRoomStore;

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
