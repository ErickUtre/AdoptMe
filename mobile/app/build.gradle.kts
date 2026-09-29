import com.google.protobuf.gradle.id
import com.google.protobuf.gradle.proto

plugins {
    alias(libs.plugins.android.application)
    alias(libs.plugins.protobuf)
}

val hostServidor = providers.gradleProperty("adoptme.servidor.host").get()
val puertoHttp = providers.gradleProperty("adoptme.servidor.puertoHttp").get()
val puertoGrpc = providers.gradleProperty("adoptme.servidor.puertoGrpc").get()

android {
    namespace = "com.adoptme.movil"
    compileSdk = 35

    defaultConfig {
        applicationId = "com.adoptme.movil"
        minSdk = 26
        targetSdk = 35
        versionCode = 2
        versionName = "2.0.0"

        buildConfigField("String", "URL_API", "\"http://$hostServidor:$puertoHttp/api/\"")
        buildConfigField("String", "HOST_GRPC", "\"$hostServidor\"")
        buildConfigField("int", "PUERTO_GRPC", puertoGrpc)
    }

    buildTypes {
        debug {
            manifestPlaceholders["permitirTextoPlano"] = true
        }
        release {
            isMinifyEnabled = false
            manifestPlaceholders["permitirTextoPlano"] = false
        }
    }

    compileOptions {
        sourceCompatibility = JavaVersion.VERSION_17
        targetCompatibility = JavaVersion.VERSION_17
    }

    buildFeatures {
        viewBinding = true
        buildConfig = true
    }

    sourceSets {
        getByName("main") {
            proto {
                srcDir("${rootDir.parentFile}/protos")
            }
        }
    }

    packaging {
        resources {
            excludes += setOf("META-INF/INDEX.LIST", "META-INF/io.netty.versions.properties")
        }
    }
}

protobuf {
    protoc {
        artifact = "com.google.protobuf:protoc:${libs.versions.protobuf.get()}"
    }
    plugins {
        id("grpc") {
            artifact = "io.grpc:protoc-gen-grpc-java:${libs.versions.grpc.get()}"
        }
    }
    generateProtoTasks {
        all().forEach { tarea ->
            tarea.builtins {
                id("java") {
                    option("lite")
                }
            }
            tarea.plugins {
                id("grpc") {
                    option("lite")
                }
            }
        }
    }
}

dependencies {
    implementation(libs.androidx.appcompat)
    implementation(libs.androidx.core)
    implementation(libs.androidx.activity)
    implementation(libs.androidx.fragment)
    implementation(libs.androidx.lifecycle.viewmodel)
    implementation(libs.androidx.lifecycle.livedata)
    implementation(libs.androidx.recyclerview)
    implementation(libs.android.material)
    implementation(libs.retrofit)
    implementation(libs.retrofit.gson)
    implementation(libs.okhttp)
    implementation(libs.osmdroid)
    implementation(libs.mpandroidchart)
    implementation(libs.play.services.location)
    implementation(libs.glide)
    annotationProcessor(libs.glide.compiler)
    implementation(libs.media3.exoplayer)
    implementation(libs.media3.ui)
    implementation(libs.grpc.okhttp)
    implementation(libs.grpc.protobuf.lite)
    implementation(libs.grpc.stub)
    implementation(libs.protobuf.javalite)
    compileOnly(libs.javax.annotation)

    testImplementation(libs.junit)
    testImplementation(libs.androidx.core.testing)
}
