import React from "react";
import { observer, inject } from "mobx-react";

import Section from "@appserver/common/components/Section";

import { showLoader, hideLoader } from "@appserver/common/utils";

import SectionHeaderContent from "./Section/Header";
import Bar from "./Section/Bar";
import SectionFilterContent from "./Section/Filter";
import SectionBodyContent from "./Section/Body";
import RoomsFilter from "@appserver/common/api/rooms/filter";

const VirtualRooms = ({
  setFirstLoad,
  setIsLoading,
  viewAs,

  rooms,
  fetchRooms,
}) => {
  const getRooms = React.useCallback(async () => {
    setIsLoading(true);

    const filterObj = RoomsFilter.getFilter(window.location);

    const searchArea = !!filterObj ? filterObj.searchArea : null;

    fetchRooms(searchArea, filterObj).then(() => {
      setIsLoading(false);
      setFirstLoad(false);
    });
  }, [fetchRooms, setIsLoading, setFirstLoad]);

  React.useEffect(() => {
    getRooms();
  }, [getRooms]);

  return (
    <Section viewAs={viewAs}>
      <Section.SectionHeader>
        <SectionHeaderContent />
      </Section.SectionHeader>

      <Section.SectionBar>
        <Bar />
      </Section.SectionBar>

      <Section.SectionFilter>
        <SectionFilterContent />
      </Section.SectionFilter>

      <Section.SectionBody>
        <SectionBodyContent />
      </Section.SectionBody>
    </Section>
  );
};

export default inject(({ filesStore, roomsStore }) => {
  const {
    setFirstLoad,
    setIsLoading,

    viewAs,
  } = filesStore;

  const { rooms, fetchRooms } = roomsStore;

  return {
    setIsLoading,
    setFirstLoad,
    viewAs,
    rooms,
    fetchRooms,
  };
})(observer(VirtualRooms));
