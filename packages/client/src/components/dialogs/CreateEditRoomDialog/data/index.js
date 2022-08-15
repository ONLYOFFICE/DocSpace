import { RoomsType } from "@docspace/common/constants";

export const roomTypes = [
  {
    type: RoomsType.FillingFormsRoom,
    title: "FillingFormsRoomTitle",
    description: "FillingFormsRoomDescription",
    defaultTag: "Files:FillingFormRooms",
    withSecondaryInfo: true,
    secondaryInfo: "FillingFormsRoomSecondaryInfo",
  },
  {
    type: RoomsType.EditingRoom,
    title: "CollaborationRoomTitle",
    description: "CollaborationRoomDescription",
    defaultTag: "Files:CollaborationRooms",
    withSecondaryInfo: true,
    secondaryInfo: "CollaborationRoomSecondaryInfo",
  },
  {
    type: RoomsType.ReviewRoom,
    title: "ReviewRoomTitle",
    description: "ReviewRoomDescription",
    defaultTag: "Files:ReviewRooms",
    withSecondaryInfo: true,
    secondaryInfo: "ReviewRoomSecondaryInfo",
  },
  {
    type: RoomsType.ReadOnlyRoom,
    title: "ViewOnlyRoomTitle",
    description: "ViewOnlyRoomDescription",
    defaultTag: "Files:ViewOnlyRooms",
    withSecondaryInfo: true,
    secondaryInfo: "ViewOnlyRoomSecondaryInfo",
  },
  {
    type: RoomsType.CustomRoom,
    title: "CustomRoomTitle",
    description: "CustomRoomDescription",
    defaultTag: "Files:CustomRooms",
    withSecondaryInfo: false,
  },
];
