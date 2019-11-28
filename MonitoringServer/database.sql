CREATE TABLE public."Monitoring_CommonInfo"
(
    "Id" integer NOT NULL DEFAULT nextval('"Monitoring_CommonInfo_Id_seq"'::regclass),
    "ClientIp" character varying(36) COLLATE pg_catalog."default",
    "TotalRAM" integer,
    "FreeRAM" integer,
    "CPULoad" numeric(5,2),
    "CreateTime" timestamp without time zone NOT NULL DEFAULT now(),
    CONSTRAINT "Monitoring_CommonInfo_pkey" PRIMARY KEY ("Id")
);

CREATE TABLE public."Monitoring_DiskInfo"
(
    "Id" integer NOT NULL DEFAULT nextval('"Monitoring_DiskInfo_Id_seq"'::regclass),
    "InfoId" integer,
    "HDDName" character varying(32) COLLATE pg_catalog."default",
    "HDDTotalSpace" bigint,
    "HDDFreeSpace" bigint,
    CONSTRAINT "Monitoring_DiskInfo_pkey" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_MON_DisInfo_Parent" FOREIGN KEY ("InfoId")
        REFERENCES public."Monitoring_CommonInfo" ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
);