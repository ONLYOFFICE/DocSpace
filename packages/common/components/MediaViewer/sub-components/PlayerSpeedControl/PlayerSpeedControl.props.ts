export interface PlayerSpeedControlProps {
  handleSpeedChange: (speed: number) => void;
  onMouseLeave: VoidFunction;
  src?: string;
}

export type SpeedType = ["X0.5", "X1", "X1.5", "X2"];

export type SpeedRecord<T extends SpeedType> = {
  [Key in T[number]]: number;
};
