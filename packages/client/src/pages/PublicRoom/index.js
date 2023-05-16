import React, { useEffect, useState } from "react";
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
  const { isLoaded, withPaging, validatePublicRoomKey } = props;

  const [isLoad, setIsLoad] = useState(false);
  const [isLoadingRoom, setIsLoadingRoom] = useState(false);
  const [status, setStatus] = useState(ValidationResult.Invalid);

  const location = useLocation();
  const key = location.search.substring(5, location.search.length);

  const validateKey = async () => {
    setIsLoad(true);

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

  const renderPage = () => {
    switch (status) {
      case ValidationResult.Ok:
        return roomPage();
      case ValidationResult.Invalid:
        return <></>;
      case ValidationResult.Expired:
        return <></>;
      case ValidationResult.Password:
        return <RoomPassword roomKey={key} />;

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
  ) : isLoaded ? (
    roomPage()
  ) : (
    renderPage()
  );
};

export default inject(({ auth, filesStore, publicRoomStore }) => {
  const { withPaging } = auth.settingsStore;
  const { validatePublicRoomKey, isLoaded } = publicRoomStore;

  return {
    isLoaded,
    withPaging,
    validatePublicRoomKey,
  };
})(observer(PublicRoom));
