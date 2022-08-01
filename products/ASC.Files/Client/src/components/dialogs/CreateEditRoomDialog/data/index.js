import { RoomsType } from "@appserver/common/constants";

export const roomTypes = [
  {
    type: RoomsType.FillingFormsRoom,
    title: "FillingFormsRoomTitle",
    description: "FillingFormsRoomDescription",
    withSecondaryInfo: true,
  },
  {
    type: RoomsType.EditingRoom,
    title: "CollaborationRoomTitle",
    description: "CollaborationRoomDescription",
    withSecondaryInfo: true,
  },
  {
    type: RoomsType.ReviewRoom,
    title: "ReviewRoomTitle",
    description: "ReviewRoomDescription",
    withSecondaryInfo: true,
  },
  {
    type: RoomsType.ReadOnlyRoom,
    title: "ViewOnlyRoomTitle",
    description: "ViewOnlyRoomDescription",
    withSecondaryInfo: true,
  },
  {
    type: RoomsType.CustomRoom,
    title: "CustomRoomTitle",
    description: "CustomRoomDescription",
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
