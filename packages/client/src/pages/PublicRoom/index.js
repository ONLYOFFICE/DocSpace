import React, { useEffect, useState } from "react";
import { observer, inject } from "mobx-react";
import { useLocation } from "react-router-dom";
import Section from "@docspace/common/components/Section";
import Loader from "@docspace/components/loader";
import FilesFilter from "@docspace/common/api/files/filter";
import { ValidationResult } from "../../helpers/constants";

import SectionHeaderContent from "./Header";
import SectionFilterContent from "../Home/Section/Filter";
import SectionBodyContent from "../Home/Section/Body";

import RoomPassword from "./sub-components/RoomPassword";

const PublicRoom = (props) => {
  const {
    fetchFiles,
    withPaging,
    setIsLoading,
    setFirstLoad,
    validatePublicRoomKey,
  } = props;

  const [isLoad, setIsLoad] = useState(false);
  const [isLoadingRoom, setIsLoadingRoom] = useState(false);
  const [status, setStatus] = useState(ValidationResult.Invalid);

  const location = useLocation();

  const validateKey = async () => {
    setIsLoad(true);
    const key = location.search.substring(5, location.search.length);
    setIsLoadingRoom(true);
    const res = await validatePublicRoomKey(key);
    //res?.status && setStatus(res?.status);
    setStatus(3);
    setIsLoadingRoom(false);

    console.log("status res", res);
    console.log("status", res.status);

    // alert(res.status); //Status: 0 – Ok, 1 – Invalid, 2 – Expired, 3 – Required Password
  };

  useEffect(() => {
    !isLoad && validateKey();
  }, [validateKey, isLoad]);

  useEffect(() => {
    const filterObj = FilesFilter.getDefault();
    const folderId = filterObj.folder;

    fetchFiles(folderId).finally(() => {
      setIsLoading(false);
      setFirstLoad(false);
    });
  }, [fetchFiles, setIsLoading, setFirstLoad]);

  const renderPage = () => {
    switch (status) {
      case ValidationResult.Ok:
        return (
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
      case ValidationResult.Invalid:
        return <></>;
      case ValidationResult.Expired:
        return <></>;
      case ValidationResult.Password:
        return <RoomPassword />;

      default:
        return <></>;
    }
  };

  return isLoadingRoom ? (
    <Section>
      <Section.SectionBody>
        <Loader className="pageLoader" type="rombs" size="40px" />
      </Section.SectionBody>
    </Section>
  ) : (
    renderPage()
  );
};

export default inject(({ auth, filesStore, publicRoomStore }) => {
  const { withPaging } = auth.settingsStore;
  const { fetchFiles, setIsLoading, setFirstLoad } = filesStore;
  const { validatePublicRoomKey } = publicRoomStore;

  return {
    fetchFiles,
    withPaging,
    setIsLoading,
    setFirstLoad,
    validatePublicRoomKey,
  };
})(observer(PublicRoom));
