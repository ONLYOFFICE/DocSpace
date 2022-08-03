import { RoomsType } from "@docspace/common/constants";

export const roomTypes = [
  {
    type: RoomsType.FillingFormsRoom,
    title: "FillingFormsRoomTitle",
    description: "FillingFormsRoomDescription",
    defaultTag: "Files:FillingFormsRoom",
    withSecondaryInfo: true,
    secondaryInfo: "FillingFormsRoomSecondaryInfo",
  },
  {
    type: RoomsType.EditingRoom,
    title: "CollaborationRoomTitle",
    description: "CollaborationRoomDescription",
    defaultTag: "Files:EditingRoom",
    withSecondaryInfo: true,
    secondaryInfo: "CollaborationRoomSecondaryInfo",
  },
  {
    type: RoomsType.ReviewRoom,
    title: "ReviewRoomTitle",
    description: "ReviewRoomDescription",
    defaultTag: "Files:ReviewRoom",
    withSecondaryInfo: true,
    secondaryInfo: "ReviewRoomSecondaryInfo",
  },
  {
    type: RoomsType.ReadOnlyRoom,
    title: "ViewOnlyRoomTitle",
    description: "ViewOnlyRoomDescription",
    defaultTag: "Files:ReadOnlyRoom",
    withSecondaryInfo: true,
    secondaryInfo: "ViewOnlyRoomSecondaryInfo",
  },
  {
    type: RoomsType.CustomRoom,
    title: "CustomRoomTitle",
    description: "CustomRoomDescription",
    defaultTag: "Files:CustomRoom",
    withSecondaryInfo: false,
  },
];

export const thirparties = [
  {
    id: 1,
    title: "Onlyoffice DocSpace",
  },
  {
    id: 2,
    title: "DropBox",
  },
  {
    id: 3,
    title: "Google Drive",
  },
  {
    id: 4,
    title: "OneDrive",
  },
  {
    id: 5,
    title: "Nextcloud",
  },
  {
    id: 6,
    title: "Yandex Disk",
  },
];
