export type MatchItemDto = {
  rank: number;
  similarity: number;
  score: number | null;

  jobChunk: string;
  resumeChunk: string;

  explanation: string | null;
  matchedSkills: string[] | null;
  missingSkills: string[] | null;
};

export type MatchMetaDto = {
  jobChunkCount: number;
  resumeChunkCount: number;
  processingTimeMs: number;
  pipelineVersion: string;
};

export type MatchResultDto = {
  documentId: string;
  topK: number;
  usedLlm: boolean;

  overallScorePercent: number; 
  summary: string | null;

  matches: MatchItemDto[];
  meta: MatchMetaDto;
};

export type RunMatchParams = {
  documentId: string;
  jobText: string;
  topK: number;
  useLlm: boolean;
};