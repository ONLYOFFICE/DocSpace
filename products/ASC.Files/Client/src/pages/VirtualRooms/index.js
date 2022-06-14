import React from "react";
import { observer, inject } from "mobx-react";

import Section from "@appserver/common/components/Section";

import { showLoader, hideLoader } from "@appserver/common/utils";

import SectionHeaderContent from "./Section/Header";
import Bar from "./Section/Bar";
import SectionFilterContent from "./Section/Filter";
import SectionBodyContent from "./Section/Body";

const fakeData = [
  {
    key: "0",
    icon: "",
    label: "Script",
    type: "view",
    tags: [
      {
        key: "0",
        label: "Review",
      },
    ],
    users: [
      { key: 0, label: "First User" },
      { key: 1, label: "Second User" },
      { key: 2, label: "Third User" },
      { key: 3, label: "Fourth User" },
      { key: 4, label: "Fifth User" },
      { key: 5, label: "Sixth User" },
      { key: 6, label: "Seventh User" },
    ],
    badge: "1",
    isPinned: true,
  },
  {
    key: "1",
    icon: "",
    label: "Script",
    type: "custom",
    tags: [
      {
        key: "0",
        label: "Review",
      },
      {
        key: "1",
        label: "Review 1",
      },
      {
        key: "2",
        label: "Review 2",
      },
      {
        key: "3",
        label: "Review 3",
      },
    ],
    users: [{ key: 0, label: "First User" }],
    badge: "10",
  },
  {
    key: "2",
    icon: "",
    label: "Fill form",
    type: "review",
    tags: [
      {
        key: "0",
        label: "Review",
      },
      {
        key: "1",
        label: "Review 1 long",
      },
      {
        key: "2",
        label: "Review 2 looooong",
      },
      {
        key: "3",
        label: "Review 3",
      },
    ],
    users: [{ key: 0, label: "First User" }],
    badge: "10",
  },
  {
    key: "3",
    icon: "",
    label: "Some name",
    type: "fill",
    tags: [
      {
        key: "0",
        label: "Review lloooooooooong",
      },
      {
        key: "1",
        label: "Review 1",
      },
      {
        key: "2",
        label: "Review 2",
      },
      {
        key: "3",
        label: "Review 3",
      },
    ],
    users: [{ key: 0, label: "First User" }],
    badge: "10",
  },
  {
    key: "4",
    icon: "",
    label: "Name for room with long name",
    type: "editing",
    tags: [
      {
        key: "0",
        label: "Review sojkddddddddddddddddddddddddddddddddddd",
      },
    ],
    users: [{ key: 0, label: "First User" }],
    badge: "10",
  },
  {
    key: "5",
    icon: "",
    label: "Script",
    type: "view",
    tags: [
      {
        key: "0",
        label: "Review",
      },
    ],
    users: [
      { key: 0, label: "First User" },
      { key: 1, label: "Second User" },
      { key: 2, label: "Third User" },
      { key: 3, label: "Fourth User" },
      { key: 4, label: "Fifth User" },
      { key: 5, label: "Sixth User" },
      { key: 6, label: "Seventh User" },
    ],
    badge: "1",
    isPinned: true,
    isPrivacy: true,
  },
  {
    key: "6",
    icon: "",
    label: "Script",
    type: "custom",
    tags: [
      {
        key: "0",
        label: "Review 111111111111111111111111111111111111111111111111",
      },
      {
        key: "1",
        label: "Review 1",
      },
    ],
    users: [{ key: 0, label: "First User" }],
    badge: "10",
    isPrivacy: true,
  },
  {
    key: "7",
    icon: "",
    label: "Fill form",
    type: "review",
    tags: [
      {
        key: "0",
        label: "Review",
      },
      {
        key: "1",
        label: "Review longsssssssssssssssssssss",
      },
    ],
    users: [{ key: 0, label: "First User" }],
    badge: "10",
    isPrivacy: true,
  },
  {
    key: "8",
    icon: "",
    label: "Some name",
    type: "fill",
    tags: [
      {
        key: "0",
        label: "Review",
      },
    ],
    users: [{ key: 0, label: "First User" }],
    badge: "10",
    isPrivacy: true,
  },
  {
    key: "9",
    icon: "",
    label: "Name for room with long name",
    type: "editing",
    tags: [
      {
        key: "0",
        label: "Review",
      },
    ],
    users: [{ key: 0, label: "First User" }],
    badge: "10",
    isPrivacy: true,
  },
  {
    key: "10",
    icon: "",
    label: "Name for room with long name",
    type: "archive",
    tags: [],
    users: [{ key: 0, label: "First User" }],
    badge: "10",
    isPrivacy: true,
  },
  {
    key: "11",
    icon: "",
    label: "Name for room with long name",
    type: "archive",
    tags: [
      {
        key: "0",
        label: "Review 111111111111111111111111",
      },
      {
        key: "1",
        label: "Review 111111111111111111222222222s",
      },
    ],
    users: [{ key: 0, label: "First User" }],
    badge: "10",
    isPrivacy: false,
  },
];

const VirtualRooms = ({ setFirstLoad, setIsLoading, viewAs }) => {
  const isEmpty = !fakeData;

  React.useEffect(() => {
    setIsLoading(false);
    setFirstLoad(false);
  }, [setFirstLoad, setIsLoading]);

  return (
    <Section viewAs={viewAs}>
      <Section.SectionHeader>
        <SectionHeaderContent />
      </Section.SectionHeader>
      <Section.SectionBar>
        <Bar />
      </Section.SectionBar>
      {!isEmpty && (
        <Section.SectionFilter>
          <SectionFilterContent />
        </Section.SectionFilter>
      )}

      <Section.SectionBody>
        <SectionBodyContent isEmpty={isEmpty} data={fakeData} />
      </Section.SectionBody>
    </Section>
  );
};

export default inject(({ filesStore }) => {
  const {
    firstLoad,
    setFirstLoad,
    isLoading,
    setIsLoading,
    viewAs,
  } = filesStore;

  if (!firstLoad) {
    if (isLoading) {
      showLoader();
    } else {
      hideLoader();
    }
  }

  return { setIsLoading, setFirstLoad, viewAs };
})(observer(VirtualRooms));
